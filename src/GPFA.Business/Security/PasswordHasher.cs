using System;
using System.Security.Cryptography;
using System.Text;

namespace GPFA.Business.Security
{
    /// <summary>
    /// Hash de senhas com PBKDF2-HMAC-SHA256. Formato armazenado:
    /// iteracoes.saltBase64.hashBase64. A comparação é feita em tempo constante.
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iteracoes = 120000;
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;

        public static string Hash(string senha)
        {
            if (string.IsNullOrEmpty(senha))
                throw new ArgumentException("A senha não pode ser vazia.", "senha");

            var salt = new byte[TamanhoSalt];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, Iteracoes, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(TamanhoHash);
            }

            return Iteracoes + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool Verificar(string senha, string hashArmazenado)
        {
            if (string.IsNullOrEmpty(senha) || string.IsNullOrWhiteSpace(hashArmazenado))
                return false;

            var partes = hashArmazenado.Split('.');
            if (partes.Length != 3) return false;

            int iteracoes;
            if (!int.TryParse(partes[0], out iteracoes) || iteracoes < 1000)
                return false;

            byte[] salt;
            byte[] hashEsperado;
            try
            {
                salt = Convert.FromBase64String(partes[1]);
                hashEsperado = Convert.FromBase64String(partes[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] hashCalculado;
            try
            {
                using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, iteracoes, HashAlgorithmName.SHA256))
                {
                    hashCalculado = pbkdf2.GetBytes(hashEsperado.Length);
                }
            }
            catch (ArgumentException)
            {
                return false;
            }

            return ComparacaoTempoConstante(hashCalculado, hashEsperado);
        }

        /// <summary>Hash descartável usado para igualar o tempo de resposta em logins inexistentes.</summary>
        public const string HashFicticio =
            "120000.5m3C3kTaxPpViKg7ZwzmvQ==.hJlLqs3dWXuz2Hibprf3BiWgzwQEkzVEyMfL/z0gG8o=";

        public static void ValidarForca(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
                throw new Models.ValidacaoException("A senha deve ter ao menos 8 caracteres.");
            if (senha.Length > 100)
                throw new Models.ValidacaoException("A senha deve ter no máximo 100 caracteres.");

            bool temMaiuscula = false, temMinuscula = false, temNumero = false;
            foreach (var c in senha)
            {
                if (char.IsUpper(c)) temMaiuscula = true;
                else if (char.IsLower(c)) temMinuscula = true;
                else if (char.IsDigit(c)) temNumero = true;
            }

            if (!temMaiuscula || !temMinuscula || !temNumero)
                throw new Models.ValidacaoException(
                    "A senha deve conter letras maiúsculas, minúsculas e ao menos um número.");
        }

        private static bool ComparacaoTempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diferenca = 0;
            for (int i = 0; i < a.Length; i++)
                diferenca |= a[i] ^ b[i];
            return diferenca == 0;
        }
    }
}
