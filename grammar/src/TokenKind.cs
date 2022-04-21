
using System;
using System.Threading;

static class TokenIDGenerator
{
    public const int ERROR_ID = -2;
    public const int EPSILON_ID = -3;
    public const int END_ID = -1;

    public const int FIRST_TERMINAL_ID = -4;
    public const int FIRST_NON_TERMINAL_ID = 1;

    private static int nextTerminalID = FIRST_TERMINAL_ID + 1;
    private static int nextNonTerminalID = FIRST_NON_TERMINAL_ID - 1;

    public static int GetNewTerminalID()
    {
        return Interlocked.Decrement(ref nextTerminalID);
    }

    public static int GetNewNonTerminalID() 
    {
        return Interlocked.Increment(ref nextNonTerminalID);
    }
}


public struct TokenKind : IEquatable<TokenKind>, IComparable<TokenKind>
{
    private TokenKind(int id)
    {
        Value = id;
    }

    public int Value { get; }
    public bool IsTerminal => Value < 0;

    public static TokenKind Epsilon = new TokenKind(TokenIDGenerator.EPSILON_ID);
    public static TokenKind End = new TokenKind(TokenIDGenerator.END_ID);
    public static TokenKind Error = new TokenKind(TokenIDGenerator.ERROR_ID);

    public static TokenKind CreateTerminal() => new TokenKind(TokenIDGenerator.GetNewTerminalID());

    public static TokenKind CreateNonTerminal() => new TokenKind(TokenIDGenerator.GetNewNonTerminalID());

    public override bool Equals(object? obj)
    {
        return obj is TokenKind iD &&
               Value == iD.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public override string ToString()
    {
        if (IsTerminal)
            return $"Terminal({-Value})";
        else
            return $"NonTerminal({Value})";
    }

    public bool Equals(TokenKind other)
    {
        return this == other;
    }

    public int CompareTo(TokenKind other)
    {
        return this.Value - other.Value;
    }

    public static bool operator ==(TokenKind left, TokenKind right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(TokenKind left, TokenKind right)
    {
        return left.Value != right.Value;
    }
}