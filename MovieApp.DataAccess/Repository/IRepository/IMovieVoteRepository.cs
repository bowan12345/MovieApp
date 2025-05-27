using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository.IRepository
{
    public interface IMovieVoteRepository : IRepository<MovieVote>
    {
        MovieVote GetUserVoteForMovie(int movieId, string userId);
        void Update(MovieVote obj);
        (int likeCount, int dislikeCount) GetMovieVoteCounts(int movieId);
    }
}