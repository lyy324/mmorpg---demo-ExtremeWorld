using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        /// <summary>
        /// 线程同步锁：保护 DbContext 在多线程环境下安全访问
        /// </summary>
        private readonly object dbLock = new object();

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
            EnsureCharacterItemsTable();
        }

        private void EnsureCharacterItemsTable()
        {
            // Older local databases may predate the CharacterItems entity.
            const string sql = @"
IF OBJECT_ID(N'dbo.CharacterItems', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CharacterItems] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [CharacterID] int NOT NULL,
        [ItemID] int NOT NULL,
        [ItemCount] int NOT NULL
    );
END;

DECLARE @characterItemsId int = OBJECT_ID(N'dbo.CharacterItems', N'U');

IF NOT EXISTS
(
    SELECT 1 FROM sys.key_constraints
    WHERE parent_object_id = @characterItemsId AND type = 'PK'
)
BEGIN
    ALTER TABLE [dbo].[CharacterItems]
        ADD CONSTRAINT [PK_CharacterItems_Auto]
        PRIMARY KEY CLUSTERED ([Id] ASC);
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE parent_object_id = @characterItemsId
)
BEGIN
    ALTER TABLE [dbo].[CharacterItems]
        ADD CONSTRAINT [FK_CharacterItem_Auto]
        FOREIGN KEY ([CharacterID]) REFERENCES [dbo].[Characters]([ID]);
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = @characterItemsId AND name = N'IX_FK_CharacterItem_Auto'
)
BEGIN
    CREATE INDEX [IX_FK_CharacterItem_Auto]
        ON [dbo].[CharacterItems]([CharacterID]);
END";

            entities.Database.ExecuteSqlCommand(sql);
        }

        public void Save(bool async = false)
        {
            lock (dbLock)
            {
                if (async)
                    entities.SaveChangesAsync();
                else
                    entities.SaveChanges();
            }
        }

        /// <summary>
        /// 线程安全地执行数据库查询操作
        /// </summary>
        public T Execute<T>(Func<ExtremeWorldEntities, T> func)
        {
            lock (dbLock)
            {
                return func(entities);
            }
        }

        /// <summary>
        /// 线程安全地执行数据库写操作
        /// </summary>
        public void Execute(Action<ExtremeWorldEntities> action)
        {
            lock (dbLock)
            {
                action(entities);
            }
        }

        /// <summary>
        /// Removes entities left in Added state after a failed transaction.
        /// </summary>
        public void DiscardPendingChanges()
        {
            lock (dbLock)
            {
                foreach (var entry in entities.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added).ToList())
                {
                    entry.State = EntityState.Detached;
                }
            }
        }
    }
}
