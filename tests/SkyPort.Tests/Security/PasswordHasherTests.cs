using SkyPort.Infrastructure.Services;

namespace SkyPort.Tests.Security;

public class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ProducesDifferentValuesForSamePassword()
    {
        var firstHash = _hasher.Hash("correct horse battery staple");
        var secondHash = _hasher.Hash("correct horse battery staple");

        Assert.NotEqual(firstHash, secondHash);
    }

    [Fact]
    public void Verify_AcceptsCorrectPassword()
    {
        const string password = "correct horse battery staple";
        var hash = _hasher.Hash(password);

        Assert.True(_hasher.Verify(password, hash));
    }

    [Fact]
    public void Verify_RejectsIncorrectPassword()
    {
        var hash = _hasher.Hash("correct horse battery staple");

        Assert.False(_hasher.Verify("incorrect password", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("v1.not-an-iteration-count.c2FsdA==.aGFzaA==")]
    [InlineData("v1.310000.not-base64!.aGFzaA==")]
    [InlineData("v2.310000.c2FsdA==.aGFzaA==")]
    public void Verify_RejectsInvalidHashFormat(string hash)
    {
        Assert.False(_hasher.Verify("any password", hash));
    }
}
