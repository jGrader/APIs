using System.Data.Entity;

namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GradeRepository : GenericRepository<GradeModel>, IGradeRepository
    {

        public GradeRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<GradeModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(g => g.UserId == userId));
            return searchResult;
        }
        public async Task<IEnumerable<GradeModel>> GetByGraderId(int graderId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(g => g.GraderId == graderId));
            return searchResult;
        }
        public async Task<IEnumerable<GradeModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(g => g.EntityId == entityId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> AddToTeam(GradeModel grade, IEnumerable<UserModel> teamMembers)
        {
            if (grade == null)
            {
                throw new ArgumentNullException("grade");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var addedGrades = new List<GradeModel>();
                    foreach (var newGrade in teamMembers.Select(teamMember => new GradeModel { UserId = teamMember.Id, Grade = grade.Grade, 
                        BonusGrade = grade.BonusGrade, EntityId = grade.EntityId, GraderId = grade.GraderId, TimeStamp = grade.TimeStamp }))
                    {
                        Context.Entry(newGrade).State = EntityState.Added;
                        addedGrades.Add(newGrade);
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return addedGrades;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public async Task<IEnumerable<GradeModel>> UpdateForTeam(GradeModel grade, IEnumerable<UserModel> teamMembers)
        {
            if (grade == null)
            {
                throw new ArgumentNullException("grade");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var returnValues = new List<GradeModel>();
                    foreach (var teamMember in teamMembers)
                    {
                        var member = teamMember;
                        var existingGrade = await DbSet.FirstOrDefaultAsync(f => f.EntityId == grade.EntityId && f.UserId == member.Id);
                        if (existingGrade == null)
                        {
                            var newGrade = new GradeModel
                            {
                                UserId = teamMember.Id,
                                Grade = grade.Grade,
                                BonusGrade = grade.BonusGrade,
                                EntityId = grade.EntityId,
                                GraderId = grade.GraderId,
                                TimeStamp = grade.TimeStamp
                            };
                            Context.Entry(newGrade).State = EntityState.Added;
                            returnValues.Add(newGrade);
                            continue;
                        }

                        existingGrade.BonusGrade = grade.BonusGrade;
                        existingGrade.EntityId = grade.EntityId;
                        existingGrade.Grade = grade.Grade;
                        existingGrade.GraderId = grade.GraderId;
                        existingGrade.TimeStamp = grade.TimeStamp;
                        existingGrade.UserId = grade.UserId;
                        Context.Entry(existingGrade).State = EntityState.Modified;
                        returnValues.Add(existingGrade);
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return returnValues;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public async Task<bool> DeleteForTeam(GradeModel existingGrade, IEnumerable<UserModel> teamMembers)
        {
            if (existingGrade == null)
            {
                throw new ArgumentNullException("existingGrade");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var teamMember in teamMembers)
                    {
                        var member = teamMember;
                        var oldGrade = await DbSet.FirstOrDefaultAsync(g => g.EntityId == existingGrade.EntityId && g.UserId == member.Id);
                        if (oldGrade != null)
                        {
                            Context.Entry(oldGrade).State = EntityState.Deleted;
                        }
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return false;
                }
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

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
        }
    }
}
