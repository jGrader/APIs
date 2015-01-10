namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Linq;
    using System.Threading.Tasks;

    public class TeamRepository : ITeamRepository
    {
        private DatabaseContext _db = new DatabaseContext();

      
        public async Task<TeamModel> Get(int teamId)
        {
            var searchResult = await _db.Team.FirstOrDefaultAsync(e => e.Id == teamId);
            return searchResult;
        }

        public async Task<IEnumerable<TeamModel>> GetAll()
        {
            return await Task.Run(() => _db.Team);
        }
        public async Task<IEnumerable<TeamModel>> GetAllForEntity(int entityId)
        {
            var searchResult = await Task.Run(() => _db.Team.Where(e => e.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<TeamModel>> GetAllForCourse(int courseId)
        {
            var searchResult = await Task.Run(() => _db.Team.Where(e => e.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public async Task<TeamModel> Add(TeamModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Team.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Entity).Load();
                _db.Entry(item).Collection(c => c.TeamMembers).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<TeamModel> Update(TeamModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.Team.FirstOrDefaultAsync(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Entry(dbItem).CurrentValues.SetValues(item);
                await _db.SaveChangesAsync();
                return await Get(item.Id);
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<bool> Delete(int teamId)
        {
            var item = await _db.Team.FirstOrDefaultAsync(c => c.Id == teamId);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Team.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_db == null)
            {
                return;
            }

            _db.Dispose();
            _db = null;
        }  
    }
}
