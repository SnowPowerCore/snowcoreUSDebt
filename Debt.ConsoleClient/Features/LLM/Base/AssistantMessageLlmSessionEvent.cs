namespace Debt.ConsoleClient.Features.LLM.Base;

public sealed record AssistantMessageLlmSessionEvent(string Content) : LlmSessionEvent;
