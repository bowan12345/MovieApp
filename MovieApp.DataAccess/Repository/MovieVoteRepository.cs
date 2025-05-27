using MovieApp.DataAccess.Data;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository
{
    public class MovieVoteRepository : Repository<MovieVote>, IMovieVoteRepository
    {
        private ApplicationDbContext _db;

        public MovieVoteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public MovieVote GetUserVoteForMovie(int movieId, string userId)
        {
            return _db.MovieVotes.FirstOrDefault(v => v.MovieId == movieId && v.UserId == userId);
        }

        public void Update(MovieVote obj)
        {
            _db.MovieVotes.Update(obj);
        }

        public (int likeCount, int dislikeCount) GetMovieVoteCounts(int movieId)
        {
            var votes = _db.MovieVotes.Where(v => v.MovieId == movieId);
            var likeCount = votes.Count(v => v.IsLike == true);
            var dislikeCount = votes.Count(v => v.IsLike == false);

            return (likeCount, dislikeCount);
        }
    }
}