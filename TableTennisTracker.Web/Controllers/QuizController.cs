using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTennisTracker.Web.Controllers;

public class QuizController : Controller
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IMatchRepository _matchRepository;

    public QuizController(
        ITournamentRepository tournamentRepository,
        IPlayerRepository playerRepository,
        IVenueRepository venueRepository,
        IMatchRepository matchRepository)
    {
        _tournamentRepository = tournamentRepository;
        _playerRepository = playerRepository;
        _venueRepository = venueRepository;
        _matchRepository = matchRepository;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await GetQuizQuestions();
        return View(questions);
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await GetQuizQuestions();
        return Json(questions);
    }

    [HttpPost]
    public IActionResult SubmitAnswer([FromBody] AnswerSubmission submission)
    {
        return Json(new { isCorrect = true });
    }

    private async Task<List<QuizQuestion>> GetQuizQuestions()
    {
        var questions = new List<QuizQuestion>();

        questions.Add(new QuizQuestion
        {
            Id = Guid.NewGuid(),
            Category = "Tournament",
            Text = "What is the primary goal of organizing a table tennis tournament?",
            Answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), Text = "To determine the champion" },
                new Answer { Id = Guid.NewGuid(), Text = "To earn money" },
                new Answer { Id = Guid.NewGuid(), Text = "To have fun with friends" }
            },
            CorrectAnswerId = 0,
            Explanation = "Determining a champion through competitive play is the primary goal."
        });

        questions.Add(new QuizQuestion
        {
            Id = Guid.NewGuid(),
            Category = "Rules",
            Text = "In table tennis, what is the standard winning score in a set?",
            Answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), Text = "11 points" },
                new Answer { Id = Guid.NewGuid(), Text = "15 points" },
                new Answer { Id = Guid.NewGuid(), Text = "21 points" }
            },
            CorrectAnswerId = 0,
            Explanation = "A player must win by at least 2 points with a minimum score of 11."
        });

        questions.Add(new QuizQuestion
        {
            Id = Guid.NewGuid(),
            Category = "Player",
            Text = "How many points does a player need to lead by to win at deuce (10-10)?",
            Answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), Text = "1 point" },
                new Answer { Id = Guid.NewGuid(), Text = "2 points" },
                new Answer { Id = Guid.NewGuid(), Text = "3 points" }
            },
            CorrectAnswerId = 1,
            Explanation = "When the score reaches 10-10 (deuce), a player must win by 2 points."
        });

        questions.Add(new QuizQuestion
        {
            Id = Guid.NewGuid(),
            Category = "Venue",
            Text = "What is the standard height of a table tennis net?",
            Answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), Text = "15.25 cm" },
                new Answer { Id = Guid.NewGuid(), Text = "20.32 cm" },
                new Answer { Id = Guid.NewGuid(), Text = "25.40 cm" }
            },
            CorrectAnswerId = 1,
            Explanation = "The net height is exactly 20.32 cm (8 inches) according to official rules."
        });

        questions.Add(new QuizQuestion
        {
            Id = Guid.NewGuid(),
            Category = "Rules",
            Text = "How many sets does a player need to win in a best-of-3 match?",
            Answers = new List<Answer>
            {
                new Answer { Id = Guid.NewGuid(), Text = "2 sets" },
                new Answer { Id = Guid.NewGuid(), Text = "3 sets" },
                new Answer { Id = Guid.NewGuid(), Text = "First to 11 points" }
            },
            CorrectAnswerId = 0,
            Explanation = "In a best-of-3 match, the first player to win 2 sets wins the match."
        });

        return await Task.FromResult(questions);
    }
}

public class QuizQuestion
{
    public Guid Id { get; set; }
    public string Category { get; set; }
    public string Text { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public int CorrectAnswerId { get; set; }
    public string Explanation { get; set; }
}

public class Answer
{
    public Guid Id { get; set; }
    public string Text { get; set; }
}

public class AnswerSubmission
{
    public Guid QuestionId { get; set; }
    public Guid SelectedAnswerId { get; set; }
}
