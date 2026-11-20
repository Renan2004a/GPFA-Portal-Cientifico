<#
.SYNOPSIS
    Cria/recria o banco do GPFA no SQL Server LocalDB.

.DESCRIPTION
    Executa, na ordem, os scripts de criação, estrutura, visões, dados de
    demonstração e usuários. Requer sqlcmd (instalado com o Visual Studio ou
    SQL Server).

.PARAMETER Database
    Nome do banco. Padrão: GPFA_Portal.

.PARAMETER Recreate
    Remove o banco antes de recriá-lo. Use com atenção: apaga os dados.

.EXAMPLE
    .\scripts\Setup-Database.ps1 -Recreate
#>
[CmdletBinding()]
param(
    [string]$Database = 'GPFA_Portal',
    [string]$Server = '(localdb)\MSSQLLocalDB',
    [switch]$Recreate,
    [switch]$NoSeed
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$dbFolder = Join-Path $root 'database'

function Invoke-SqlScript {
    param([string]$Path)
    Write-Host "  -> $(Split-Path -Leaf $Path)" -ForegroundColor Cyan
    & sqlcmd -S $Server -b -f 65001 -i $Path -v DbName="$Database"
    if ($LASTEXITCODE -ne 0) {
        throw "Falha ao executar $Path (exit code $LASTEXITCODE)."
    }
}

Write-Host "GPFA - preparação do banco '$Database' em '$Server'" -ForegroundColor Green

if ($Recreate) {
    Invoke-SqlScript (Join-Path $dbFolder '99-drop-database.sql')
}

Invoke-SqlScript (Join-Path $dbFolder '00-create-database.sql')
Invoke-SqlScript (Join-Path $dbFolder '01-schema.sql')
Invoke-SqlScript (Join-Path $dbFolder '02-views.sql')

if (-not $NoSeed) {
    Invoke-SqlScript (Join-Path $dbFolder '03-seed.sql')
    Invoke-SqlScript (Join-Path $dbFolder '04-seed-usuarios.sql')
}

Write-Host "Banco '$Database' pronto." -ForegroundColor Green
