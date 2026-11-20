/* ============================================================================
   GPFA - Script 04 - Usuários de demonstração
   Senhas armazenadas com PBKDF2-HMAC-SHA256 (120.000 iterações, salt aleatório).
   Formato: iteracoes.saltBase64.hashBase64
   Credenciais de demonstração (documentadas no README e no manual):
     admin / Gpfa@2026          (Administrador)
     coordenador / Coord@2026   (Coordenador - Helena Marques Ferreira)
     pesquisador / Pesq@2026    (Pesquisador - Rafael Nogueira Lima)
   ============================================================================ */
:setvar DbName "GPFA_Portal"
USE [$(DbName)];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

INSERT dbo.Usuario (Login, NomeCompleto, Email, SenhaHash, PerfilId, PesquisadorId, Ativo) VALUES
 (N'admin', N'Administração GPFA', N'admin@gpfa.ufmg.br',
  N'120000.nia6vSECTtUHcZduiPAbtw==.DrQbOxFpZQ5aAXEYWdYJQpZPSQdyhA2684wPnYhqIwA=',
  1, NULL, 1),
 (N'coordenador', N'Helena Marques Ferreira', N'helena.ferreira@gpfa.ufmg.br',
  N'120000.fCyXHiystI+ZMJuKZQEbig==.LF+mjtL/0u/KAKWkCMMjy7fcjIuT2UtaFbzN0QyDhuo=',
  2, 1, 1),
 (N'pesquisador', N'Rafael Nogueira Lima', N'rafael.lima@gpfa.ufmg.br',
  N'120000.5m3C3kTaxPpViKg7ZwzmvQ==.hJlLqs3dWXuz2Hibprf3BiWgzwQEkzVEyMfL/z0gG8o=',
  3, 2, 1);
GO

PRINT N'Usuários de demonstração inseridos.';
GO
