using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Shared.DTOs;
using System.Text.RegularExpressions;

namespace QuizApp.QuizApp.API.Hubs
{
    [Authorize]
    public class QuizHub : Hub
    {
        private readonly IQuizSessionService _sessionService;
        private readonly IQuizAttemptService _attemptService;

        public QuizHub(IQuizSessionService sessionService, IQuizAttemptService attemptService)
        {
            _sessionService = sessionService;
            _attemptService = attemptService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinSession(string sessionCode, string displayName)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId == null)
                {
                    await Clients.Caller.SendAsync("Error", "User identifier is null.");
                    return;
                }

                var joinDto = new JoinSessionDto
                {
                    SessionCode = sessionCode,
                    DisplayName = displayName
                };

                var participant = await _sessionService.JoinSessionAsync(joinDto, userId);

                // Add to session group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{participant.SessionId}");

                // Notify others in the session
                await Clients.Group($"session-{participant.SessionId}")
                    .SendAsync("ParticipantJoined", participant);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task LeaveSession(int sessionId)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId == null)
                {
                    await Clients.Caller.SendAsync("Error", "User identifier is null.");
                    return;
                }

                var participant = await _sessionService.LeaveSessionAsync(sessionId, userId);

                // Remove from session group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");

                // Notify others in the session
                await Clients.Group($"session-{sessionId}")
                    .SendAsync("ParticipantLeft", participant);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task StartSession(int sessionId)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId == null)
                {
                    await Clients.Caller.SendAsync("Error", "User identifier is null.");
                    return;
                }

                var session = await _sessionService.StartSessionAsync(sessionId, userId);

                // Notify all participants that the session has started
                await Clients.Group($"session-{sessionId}")
                    .SendAsync("SessionStarted", session);

                // Get the first question
                var question = await _sessionService.GetCurrentQuestionAsync(sessionId);

                // Send the first question to all participants
                await Clients.Group($"session-{sessionId}")
                    .SendAsync("QuestionReceived", question);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task NextQuestion(int sessionId)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId == null)
                {
                    await Clients.Caller.SendAsync("Error", "User identifier is null.");
                    return;
                }

                var question = await _sessionService.MoveToNextQuestionAsync(sessionId, userId);

                if (question != null)
                {
                    // Send next question to all participants
                    await Clients.Group($"session-{sessionId}")
                        .SendAsync("QuestionReceived", question);
                }
                else
                {
                    // End of quiz
                    var session = await _sessionService.EndSessionAsync(sessionId, userId);
                    await Clients.Group($"session-{sessionId}")
                        .SendAsync("SessionEnded", session);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task SubmitAnswer(SubmitAnswerDto submitAnswerDto)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId == null)
                {
                    await Clients.Caller.SendAsync("Error", "User identifier is null.");
                    return;
                }

                var attemptResult = await _attemptService.SubmitAnswerAsync(submitAnswerDto, userId);

                // Send confirmation to the user
                await Clients.Caller.SendAsync("AnswerSubmitted", attemptResult);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }
    }
}
