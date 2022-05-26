using NUnit.Framework;
using CertificateShortcutProvider;
using System.Security.Cryptography.X509Certificates;
using KeePassLib.Security;
using KeePassLib.Utility;
using System.Text;
using System.IO;

namespace Tests
{
    /// <summary>
    /// WARNING: The following tests require the test certificate to be added to the default local user store. Password is "password".
    /// </summary>
    public class Tests
    {
        readonly X509Certificate2 _cert = new X509Certificate2(Path.Combine(TestContext.CurrentContext.TestDirectory, "testCert.p12"), "password");
        readonly ProtectedString _keepassMasterPassword = new ProtectedString(false, "keepass_master_password");
        readonly string _xmlV1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CertificateShortcutProviderKey xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" Version=""1"">
    <Certificate>MIIFozCCA4ugAwIBAgIUJMkuwzE8WhIE0TP6Mw/FHxfptLwwDQYJKoZIhvcNAQELBQAwYDELMAkGA1UEBhMCQVUxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDEZMBcGA1UEAwwQVGVzdCBDZXJ0aWZpY2F0ZTAgFw0yMjA1MjYxMzA1MjZaGA8yMTIyMDUwMjEzMDUyNlowYDELMAkGA1UEBhMCQVUxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDEZMBcGA1UEAwwQVGVzdCBDZXJ0aWZpY2F0ZTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKBA4anlb75kvmQpImHGQNQNtys1Rv/FMNKTGaH7do5HmJx3nzRxyv8eML9Sf1JWALdocnEEVnq0hJdrRZ5S7dHK3MKpSJkEb35s3stpAi3d2HQ5VppD5gtNVC2Wwfr8GN3xVXf53ucJPDVb+TrmV6KTGWfAVdhTSQkXi4iBY5Y4bf71ipOLpuARKPnrTJE3ASZYOklzERHsxV0pbnpbmNTGZ2FxQVbyNMLv7ivlxIwWb+ckzJrnGD4BIlHLMKDu7jbiIFClpgsnf5WxruvACB/50tmTvGjgEerXSadKgNWkgPBuCRsSh/cjH8j4nwVvagJgWWpJwTPoJ0GeeiUVOkY2P7DeT174MFk8iAivpeE6Qn53rD8mCGkrg9+mQjTF6cKtgq/MM9OV4jbXgOhjUUnbCGfH/beW+1DtlmIFahCIoXPlQebsQqmglWWgkVDOyrFqCuYV4cdqr6n/fFq9FJfDy4sw8uuWqHv6M21JiSGdWdpGZ5pr9rmCes2VAp03CuzmQGQ62L5K4yxE+L7J6Qf0yV8On2duqipYpr/LbWBq4ugaGvlAF2UoJFBuRyYIHqyZdtTqS6nrjaGe8GA75SagfFHE+r+H20iTERJFdfFWP0QfUvPhDOl6a9x+jsGwHGQgAO8WeRi75VIrgufkK6po4FA409irc2k0ceysoIcRAgMBAAGjUzBRMB0GA1UdDgQWBBRFNalzNzgIMCBKsonBldYfsEd2OTAfBgNVHSMEGDAWgBRFNalzNzgIMCBKsonBldYfsEd2OTAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4ICAQAnz8kW+wsyteYCGK3F+EwN/feuQkKlPxWjl6ZGkg+xe5mgsAtNGwoWZz8EhyY3ppjTsJLxGWM9QhCSF2MeN0O0xGAQbiHpt/sjcUsDL9nb8u9aniNfjRTucEEM8HmBoxu+uKU2ZGPkz+DJllbv7rsDoVW54zaq+N/DxcOLnhZqQTwnjFmwncWaUy8u4Dz4oiDHIv8oi5v71DLfAUmIX7B7844e9VQ09xSxBCABTSyAj4LvQ3paCNjJ70362X6qprebCwIvEVcDyU2afPXdFqXI1kjTDwDBDMUB7MbbjqIo465GZr5IR5ccVxJhEkgJorwrxN6ywKb14oSQvbZhlDb+n3rOivvTJpKBoL9yLqRjq+lqD39627shf2zE4NBhtkpmaPTqg2VvN0YASuKoBswrdEi+OeLeLOniOIjbtPjN/9QPOSKOVxDE1VDyS187MsayfpuZphsMxKPvg9oDFmnFrf5kKxRTBIBGdtxNFamOWJqWLZCJ3wSp8w7QdnPAnn7sAswuI6RMh5EHsa6IN01lNw5XOd1VYDJqQW7ANZspiCJjxOpe8Dk6G7zLzD9EtlY+xPHW7/6eyTiJif1Z08lTZzqX2RgcYVL2EJiuj42FrGL6avYwnY7hN6hrnr431Zb/BY8ydpRTZh7k6zgYRK5+F+ic2NNp26DHvMRk26WsvQ==</Certificate>
    <EncryptedKey>GOgtNAM2XUm8XzJpSPX9mOdxg7MoN7zyuKGEj8f1TElgmvwgu1U3q2I4+7ukJS2YaCiDswFHqRZr8TsN3906Z1viOogKoDgEkSAktPLErF8F7+ZGcMwIv4dGoALcTLsOWMX7ViLoj12ZLyuyfuV6Unsmsxsi1K6NlzpjWDd0dqq+4l7Dbds1amQWzeFa5Pl7ZFKbUVk6LHwA1vexoLwU5dGxL8nn1pwNjemCbStjeD27teszB2FwrGZBIUzBSHQCsRQoOysBXLYi6s/+7RjtUOSUjem5hCJxY1ynGGTQSKnjXaqH3+N4Z0Zihwan6UVWVEaZvqBxruwJKPiM8qJqSJwrRrnJRWlHqOLO7HaM27a3JE4RFlu/63jFQ1hlSEWV2gXBtKIdjBhTT5+2OcNu7J2iEGn/E/oW+s8dN2u9g0ugoK2U020AQsF6XoMuDgHWYGgJ9YW6NLuvcOgpb47CYgB7IdUujirfcvp2oV9cemyckZCadC4JFiotf3rn0Gbq8o5HJK37Bb9gGzgWGcsw930ZvsbPDJOsUqoe2fyYinwp5nVseylJe0cpCf2R3uMWomXbWfE0YZ7XFKy508KwJJoH7MUtPNmCNDtLBA3CJtXstaGcGmx52TWP/0RKifB8u74yUZPgcXyUtLbamCnou1MlyKfvN+ALeFfcHRiltaU=</EncryptedKey>
    <IV>t1RiGymM5Da7Ocd7AxM/7A==</IV>
    <EncryptedPassphrase>DMASItbuZwPOczmlkvzXGVK5DhZpujllPvjtnCkI33Q=</EncryptedPassphrase>
</CertificateShortcutProviderKey>";
        readonly string _xmlV2_with_PKCS1_Padding = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CertificateShortcutProviderKey xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" Version=""2"">
    <Certificate>MIIFozCCA4ugAwIBAgIUJMkuwzE8WhIE0TP6Mw/FHxfptLwwDQYJKoZIhvcNAQELBQAwYDELMAkGA1UEBhMCQVUxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDEZMBcGA1UEAwwQVGVzdCBDZXJ0aWZpY2F0ZTAgFw0yMjA1MjYxMzA1MjZaGA8yMTIyMDUwMjEzMDUyNlowYDELMAkGA1UEBhMCQVUxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDEZMBcGA1UEAwwQVGVzdCBDZXJ0aWZpY2F0ZTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKBA4anlb75kvmQpImHGQNQNtys1Rv/FMNKTGaH7do5HmJx3nzRxyv8eML9Sf1JWALdocnEEVnq0hJdrRZ5S7dHK3MKpSJkEb35s3stpAi3d2HQ5VppD5gtNVC2Wwfr8GN3xVXf53ucJPDVb+TrmV6KTGWfAVdhTSQkXi4iBY5Y4bf71ipOLpuARKPnrTJE3ASZYOklzERHsxV0pbnpbmNTGZ2FxQVbyNMLv7ivlxIwWb+ckzJrnGD4BIlHLMKDu7jbiIFClpgsnf5WxruvACB/50tmTvGjgEerXSadKgNWkgPBuCRsSh/cjH8j4nwVvagJgWWpJwTPoJ0GeeiUVOkY2P7DeT174MFk8iAivpeE6Qn53rD8mCGkrg9+mQjTF6cKtgq/MM9OV4jbXgOhjUUnbCGfH/beW+1DtlmIFahCIoXPlQebsQqmglWWgkVDOyrFqCuYV4cdqr6n/fFq9FJfDy4sw8uuWqHv6M21JiSGdWdpGZ5pr9rmCes2VAp03CuzmQGQ62L5K4yxE+L7J6Qf0yV8On2duqipYpr/LbWBq4ugaGvlAF2UoJFBuRyYIHqyZdtTqS6nrjaGe8GA75SagfFHE+r+H20iTERJFdfFWP0QfUvPhDOl6a9x+jsGwHGQgAO8WeRi75VIrgufkK6po4FA409irc2k0ceysoIcRAgMBAAGjUzBRMB0GA1UdDgQWBBRFNalzNzgIMCBKsonBldYfsEd2OTAfBgNVHSMEGDAWgBRFNalzNzgIMCBKsonBldYfsEd2OTAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4ICAQAnz8kW+wsyteYCGK3F+EwN/feuQkKlPxWjl6ZGkg+xe5mgsAtNGwoWZz8EhyY3ppjTsJLxGWM9QhCSF2MeN0O0xGAQbiHpt/sjcUsDL9nb8u9aniNfjRTucEEM8HmBoxu+uKU2ZGPkz+DJllbv7rsDoVW54zaq+N/DxcOLnhZqQTwnjFmwncWaUy8u4Dz4oiDHIv8oi5v71DLfAUmIX7B7844e9VQ09xSxBCABTSyAj4LvQ3paCNjJ70362X6qprebCwIvEVcDyU2afPXdFqXI1kjTDwDBDMUB7MbbjqIo465GZr5IR5ccVxJhEkgJorwrxN6ywKb14oSQvbZhlDb+n3rOivvTJpKBoL9yLqRjq+lqD39627shf2zE4NBhtkpmaPTqg2VvN0YASuKoBswrdEi+OeLeLOniOIjbtPjN/9QPOSKOVxDE1VDyS187MsayfpuZphsMxKPvg9oDFmnFrf5kKxRTBIBGdtxNFamOWJqWLZCJ3wSp8w7QdnPAnn7sAswuI6RMh5EHsa6IN01lNw5XOd1VYDJqQW7ANZspiCJjxOpe8Dk6G7zLzD9EtlY+xPHW7/6eyTiJif1Z08lTZzqX2RgcYVL2EJiuj42FrGL6avYwnY7hN6hrnr431Zb/BY8ydpRTZh7k6zgYRK5+F+ic2NNp26DHvMRk26WsvQ==</Certificate>
    <EncryptedKey>T96ttNgeMP7QEn1AIngvUzHIzCb0hhXmh1ETtHfvHC212JJsHa3x2xkXTGFy2jygCzLyh6fldd3NfoZngPPJviNBtEgepcwHv9vN/cW2v8CKgyngG55ZAtvHNzbuwaAmr/BFNnWk/tg7Xpi/uS5IxAtkJ3+m5vetyYJXIFBxmeemOMQtSzSSFYV+6reRY2PdQNB7SoYp5ugoVbNAemk75cyRUvMJKmnBH5H8sCZy08XVa1e8PwJ0oBkWQCr8y6nfExXgZok1CX9261dB7x7o/shloRHO8BxyY0I62+VQ5MYaigdS6W/k/xFmhRcezIWfgydtP7eZGQj92m+cadMFNOdSM0s0XYSV+ZADdNr9ZgP7Jzjer91dIGDOdLysKkm57F24KfzB+cVibBsLF8aADDkkCq2X4DNRbYAm183mkTAA0qMKNEHlPENFstAeHrYUUWkUbx88slcSujNbU7EImKNn+zl2hosYtQCFSuSubZHA80XWLgh8PZ1ooIgcQaJCxfU0AYxhISLVA46oF+jiaBkGCrifkqMkuOumucFSR4YHZbUsFwrNmf/TXmqlyDUK0G4ou8RIBrF1BNMVzX68aS/60R0hrzgHw4ny4iIlcjB4eSX2K2mR4QMjbn5IPTUi5sW0ldnroIEmqp6uhLnsdjGQPAGHteQfHYLPAmiobrI=</EncryptedKey>
    <IV>QfrOnERAGjvO+AujMZ/vFw==</IV>
    <EncryptedPassphrase>QvW3hfxlatYJ6a/fm6kSAVEsI3HMj/d5nL6EvTgNkT4=</EncryptedPassphrase>
    <RSAEncryptionPaddingName>PKCS1</RSAEncryptionPaddingName>
</CertificateShortcutProviderKey>";

        [Test]
        public void RoundTrip_With_Default_RSAEncryption_Test()
        {
            var encryptionResult = CryptoHelpers.EncryptPassphrase(_cert, _keepassMasterPassword);

            // Serialize to xml
            string xml;
            using (var ms = new MemoryStream())
            {
                XmlUtilEx.Serialize(ms, encryptionResult);
                xml = Encoding.UTF8.GetString(ms.ToArray());
            }

            // Deserialize xml
            XmlCertificateShortcutProviderKey deserialized;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                deserialized = XmlUtilEx.Deserialize<XmlCertificateShortcutProviderKey>(ms);
            }

            var roundTripResult = CryptoHelpers.DecryptPassphrase(deserialized);

            Assert.That(roundTripResult.ReadData(), Is.EquivalentTo(_keepassMasterPassword.ReadUtf8()));

            Assert.That(encryptionResult.RSAEncryptionPadding, Is.EqualTo(AllowedRSAEncryptionPadding.Default));
            Assert.That(deserialized.RSAEncryptionPadding, Is.EqualTo(AllowedRSAEncryptionPadding.Default));
        }

        [Test]
        public void RoundTrip_V2_With_NonDefault_RSAPadding_Test()
        {
            var encryptionResult = CryptoHelpers.EncryptPassphrase(_cert, _keepassMasterPassword, AllowedRSAEncryptionPadding.List[1]);

            // Serialize to xml
            string xml;
            using (var ms = new MemoryStream())
            {
                XmlUtilEx.Serialize(ms, encryptionResult);
                xml = Encoding.UTF8.GetString(ms.ToArray());
            }

            // Deserialize xml
            XmlCertificateShortcutProviderKey deserialized;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                deserialized = XmlUtilEx.Deserialize<XmlCertificateShortcutProviderKey>(ms);
            }

            var roundTripResult = CryptoHelpers.DecryptPassphrase(deserialized);

            Assert.That(roundTripResult.ReadData(), Is.EquivalentTo(_keepassMasterPassword.ReadUtf8()));

            Assert.That(encryptionResult.RSAEncryptionPadding, Is.EqualTo(AllowedRSAEncryptionPadding.List[1]));
            Assert.That(deserialized.RSAEncryptionPadding, Is.EqualTo(AllowedRSAEncryptionPadding.List[1]));
        }

        [Test]
        public void V2_Xml_With_PKCS1_Padding_Can_Be_Decrypted()
        {
            // Deserialize xml
            XmlCertificateShortcutProviderKey deserialized;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_xmlV2_with_PKCS1_Padding)))
            {
                deserialized = XmlUtilEx.Deserialize<XmlCertificateShortcutProviderKey>(ms);
            }

            var roundTripResult = CryptoHelpers.DecryptPassphrase(deserialized);

            Assert.That(roundTripResult.ReadData(), Is.EquivalentTo(_keepassMasterPassword.ReadUtf8()));

            Assert.That(deserialized.RSAEncryptionPadding, Is.EqualTo(AllowedRSAEncryptionPadding.List[1]));
        }

        [Test]
        public void XmlKeyV1_Uses_OEAPSH256_RSAEncryptionPadding()
        {
            XmlCertificateShortcutProviderKey deserialized;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_xmlV1)))
            {
                deserialized = XmlUtilEx.Deserialize<XmlCertificateShortcutProviderKey>(ms);
            }

            Assert.That(deserialized.RSAEncryptionPadding.Name, Is.EqualTo("OAEPSHA256"));
        }

        [Test]
        public void Default_AllowedRSAEncryptionPadding_Is_OAEPSHA256()
        {
            Assert.That(AllowedRSAEncryptionPadding.Default.Name, Is.EqualTo("OAEPSHA256"));
        }
    }
}
