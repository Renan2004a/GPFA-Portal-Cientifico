"""Gera avatares e imagens abstratas dos projetos em SVG (sem dependências)."""
from pathlib import Path
import hashlib

ROOT = Path(__file__).resolve().parent.parent
AVATARS = ROOT / "src" / "GPFA.Portal" / "Content" / "img" / "avatars"
PROJETOS = ROOT / "src" / "GPFA.Portal" / "Content" / "img" / "projetos"
AVATARS.mkdir(parents=True, exist_ok=True)
PROJETOS.mkdir(parents=True, exist_ok=True)

PALETTES = [
    ("#0e3b57", "#2ec4b6"), ("#123a4f", "#e9a23b"), ("#14304a", "#7dd3c7"),
    ("#1c3b3a", "#a7e8bd"), ("#2a2f4a", "#c7b9ff"), ("#3a2a2a", "#f2b8a2"),
    ("#0f3d4a", "#8fd3f4"), ("#2b3a55", "#ffd166"),
]

PESSOAS = [
    ("helena-marques", "Helena Marques Ferreira", "HM"),
    ("rafael-lima", "Rafael Nogueira Lima", "RL"),
    ("camila-alves", "Camila Duarte Alves", "CA"),
    ("bruno-campos", "Bruno Teixeira Campos", "BC"),
    ("larissa-mendes", "Larissa Prado Mendes", "LM"),
    ("gustavo-rocha", "Gustavo Henrique Rocha", "GR"),
    ("ana-souza", "Ana Beatriz Souza", "AS"),
    ("lucas-oliveira", "Lucas Martins Oliveira", "LO"),
    ("mariana-ribeiro", "Mariana Costa Ribeiro", "MR"),
    ("pedro-almeida", "Pedro Henrique Almeida", "PA"),
    ("julia-rocha", "Julia Fernandes Rocha", "JR"),
    ("thiago-nunes", "Thiago Barbosa Nunes", "TN"),
    ("beatriz-castro", "Beatriz Almeida Castro", "BC"),
    ("gabriel-pires", "Gabriel Rocha Pires", "GP"),
    ("sofia-melo", "Sofia Andrade Melo", "SM"),
    ("henrique-tavares", "Henrique Silva Tavares", "HT"),
    ("renata-vieira", "Renata Lopes Vieira", "RV"),
    ("felipe-santos", "Felipe Andrade Santos", "FS"),
    ("isabela-dias", "Isabela Moreira Dias", "ID"),
]

PROJETOS_IMG = [
    ("materiais-quanticos", "MQ", "Materiais quânticos"),
    ("nanofotonica", "NF", "Nanofotônica"),
    ("simulacao", "SC", "Simulação"),
    ("sensores-ambientais", "SA", "Sensores ambientais"),
    ("materiais-funcionais", "MF", "Materiais funcionais"),
    ("espectroscopia", "ES", "Espectroscopia"),
]


def palette(name: str):
    h = int(hashlib.md5(name.encode()).hexdigest(), 16)
    return PALETTES[h % len(PALETTES)]


def avatar(slug, name, initials):
    bg, accent = palette(slug)
    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 400" role="img" aria-label="Retrato ilustrado de {name}">
  <defs>
    <linearGradient id="g" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0" stop-color="{bg}"/><stop offset="1" stop-color="{bg}" stop-opacity=".82"/>
    </linearGradient>
    <radialGradient id="glow" cx="30%" cy="26%" r="70%">
      <stop offset="0" stop-color="{accent}" stop-opacity=".55"/>
      <stop offset="1" stop-color="{accent}" stop-opacity="0"/>
    </radialGradient>
    <pattern id="p" width="34" height="34" patternUnits="userSpaceOnUse">
      <circle cx="2" cy="2" r="1.3" fill="#ffffff" opacity=".16"/>
    </pattern>
  </defs>
  <rect width="400" height="400" fill="url(#g)"/>
  <rect width="400" height="400" fill="url(#glow)"/>
  <rect width="400" height="400" fill="url(#p)"/>
  <g fill="none" stroke="{accent}" stroke-opacity=".38">
    <ellipse cx="200" cy="200" rx="132" ry="64" transform="rotate(-24 200 200)"/>
    <ellipse cx="200" cy="200" rx="132" ry="64" transform="rotate(38 200 200)"/>
    <circle cx="200" cy="200" r="128"/>
  </g>
  <circle cx="200" cy="200" r="104" fill="#061826" opacity=".28"/>
  <text x="200" y="200" text-anchor="middle" dominant-baseline="central"
        font-family="Georgia, serif" font-size="112" font-weight="600" fill="#ffffff" opacity=".96">{initials}</text>
  <circle cx="330" cy="70" r="9" fill="{accent}"/>
  <circle cx="74" cy="330" r="5" fill="{accent}" opacity=".8"/>
</svg>
'''


def project(slug, code, title):
    bg, accent = palette(slug + "-project")
    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 675" role="img" aria-label="{title}">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0" stop-color="{bg}"/><stop offset=".55" stop-color="{bg}" stop-opacity=".92"/>
      <stop offset="1" stop-color="#061826"/>
    </linearGradient>
    <radialGradient id="halo" cx="76%" cy="24%" r="55%">
      <stop offset="0" stop-color="{accent}" stop-opacity=".55"/>
      <stop offset="1" stop-color="{accent}" stop-opacity="0"/>
    </radialGradient>
    <pattern id="grid" width="48" height="48" patternUnits="userSpaceOnUse">
      <path d="M48 0H0V48" fill="none" stroke="#ffffff" stroke-opacity=".06"/>
    </pattern>
  </defs>
  <rect width="1200" height="675" fill="url(#bg)"/>
  <rect width="1200" height="675" fill="url(#halo)"/>
  <rect width="1200" height="675" fill="url(#grid)"/>
  <g fill="none" stroke="{accent}" stroke-opacity=".55">
    <circle cx="860" cy="300" r="170"/>
    <ellipse cx="860" cy="300" rx="260" ry="108" transform="rotate(-22 860 300)" stroke-opacity=".32"/>
    <ellipse cx="860" cy="300" rx="260" ry="108" transform="rotate(34 860 300)" stroke-opacity=".22"/>
    <circle cx="860" cy="300" r="72" fill="{accent}" fill-opacity=".12"/>
  </g>
  <g fill="{accent}">
    <circle cx="860" cy="300" r="16"/>
    <circle cx="1082" cy="198" r="8" opacity=".85"/>
    <circle cx="652" cy="382" r="5" opacity=".65"/>
    <circle cx="1010" cy="432" r="4" opacity=".55"/>
  </g>
  <g opacity=".9">
    <text x="84" y="566" font-family="Georgia, serif" font-size="150" font-weight="600" fill="#ffffff" opacity=".14">{code}</text>
    <text x="88" y="602" font-family="Inter, Segoe UI, sans-serif" font-size="21" letter-spacing="6" fill="{accent}" opacity=".9">GPFA · PESQUISA</text>
  </g>
</svg>
'''


for slug, name, initials in PESSOAS:
    (AVATARS / f"{slug}.svg").write_text(avatar(slug, name, initials), encoding="utf-8")

for slug, code, title in PROJETOS_IMG:
    (PROJETOS / f"{slug}.svg").write_text(project(slug, code, title), encoding="utf-8")

print(f"{len(PESSOAS)} avatares e {len(PROJETOS_IMG)} imagens de projeto gerados.")
