from datetime import datetime
from pathlib import Path

from fpdf import FPDF
from fpdf.enums import XPos, YPos


OUTPUT = Path(r"d:\mahdi-adem-v2\BI\test-qualite\Presentation_Hosted_Testing_2026-03-15.pdf")


class PremiumPDF(FPDF):
    def header(self):
        if self.page_no() == 1:
            return
        self.set_fill_color(241, 245, 249)
        self.rect(0, 0, 210, 14, style="F")
        self.set_text_color(15, 23, 42)
        self.set_font("Helvetica", "B", 10)
        self.cell(0, 9, "WWI BI - Rapport Qualite Heberge", 0, 1, "L", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        self.set_draw_color(226, 232, 240)
        self.line(10, 14, 200, 14)

    def footer(self):
        self.set_y(-12)
        self.set_draw_color(226, 232, 240)
        self.line(10, self.get_y(), 200, self.get_y())
        self.set_font("Helvetica", "I", 9)
        self.set_text_color(100, 116, 139)
        self.cell(0, 8, f"Page {self.page_no()}", 0, 0, "C", new_x=XPos.RIGHT, new_y=YPos.TOP)


def section_title(pdf: FPDF, text: str):
    pdf.ln(2)
    pdf.set_fill_color(15, 23, 42)
    pdf.set_text_color(248, 250, 252)
    pdf.set_font("Helvetica", "B", 13)
    pdf.cell(0, 9, f"  {text}", 0, 1, "L", fill=True, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.ln(1)


def body_line(pdf: FPDF, text: str, bold: bool = False):
    pdf.set_text_color(30, 41, 59)
    pdf.set_font("Helvetica", "B" if bold else "", 11)
    pdf.multi_cell(0, 6, text, new_x=XPos.LMARGIN, new_y=YPos.NEXT)


def bullet(pdf: FPDF, text: str):
    pdf.set_text_color(30, 41, 59)
    pdf.set_font("Helvetica", "", 11)
    pdf.multi_cell(0, 6, f"- {text}", new_x=XPos.LMARGIN, new_y=YPos.NEXT)


def kpi_card_row(pdf: FPDF):
    y = pdf.get_y()
    x0 = 12
    gap = 4
    w = (210 - 24 - gap * 2) / 3
    h = 25

    cards = [
        ("14/14", "Cas de test PASS", (16, 185, 129)),
        ("2/2", "Tests Selenium PASS", (59, 130, 246)),
        ("2/2", "Checks API NF PASS", (245, 158, 11)),
    ]

    for i, (main, sub, color) in enumerate(cards):
        x = x0 + i * (w + gap)
        pdf.set_fill_color(*color)
        pdf.rect(x, y, w, h, style="F")
        pdf.set_xy(x + 3, y + 5)
        pdf.set_text_color(255, 255, 255)
        pdf.set_font("Helvetica", "B", 16)
        pdf.cell(w - 6, 7, main, 0, 2, "L")
        pdf.set_font("Helvetica", "", 10)
        pdf.cell(w - 6, 6, sub, 0, 1, "L")

    pdf.set_xy(12, y + h + 4)


def command_block(pdf: FPDF, title: str, commands):
    body_line(pdf, title, bold=True)
    pdf.set_fill_color(248, 250, 252)
    start_y = pdf.get_y() + 1
    total_h = 6 * len(commands) + 4
    pdf.rect(12, start_y, 186, total_h, style="F")
    pdf.set_xy(15, start_y + 2)
    pdf.set_font("Courier", "", 10)
    pdf.set_text_color(15, 23, 42)
    for cmd in commands:
        pdf.multi_cell(180, 6, cmd, new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.set_x(15)
    pdf.ln(2)


def add_cover(pdf: FPDF):
    pdf.add_page()
    pdf.set_fill_color(15, 23, 42)
    pdf.rect(0, 0, 210, 297, style="F")

    pdf.set_fill_color(30, 41, 59)
    pdf.rect(12, 22, 186, 250, style="F")

    pdf.set_xy(20, 42)
    pdf.set_text_color(148, 163, 184)
    pdf.set_font("Helvetica", "B", 12)
    pdf.cell(0, 8, "PRESENTATION EXECUTIVE", 0, 1, "L")

    pdf.set_x(20)
    pdf.set_text_color(248, 250, 252)
    pdf.set_font("Helvetica", "B", 28)
    pdf.multi_cell(170, 12, "Rapport Qualite\nPlateforme BI Hebergee", new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    pdf.set_x(20)
    pdf.set_text_color(203, 213, 225)
    pdf.set_font("Helvetica", "", 12)
    pdf.multi_cell(
        170,
        7,
        "Validation complete des tests fonctionnels et non fonctionnels sur https://bi.tunibyte.com",
        new_x=XPos.LMARGIN,
        new_y=YPos.NEXT,
    )

    pdf.ln(10)
    pdf.set_x(20)
    pdf.set_text_color(248, 250, 252)
    pdf.set_font("Helvetica", "B", 12)
    pdf.cell(0, 8, "Contexte", 0, 1, "L")
    pdf.set_x(20)
    pdf.set_font("Helvetica", "", 11)
    pdf.multi_cell(170, 6, "Backend ASP.NET Core 8, Frontend Angular 17, Auth Firebase + JWT", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.set_x(20)
    pdf.multi_cell(170, 6, "Campagne de test finalisee le 2026-03-15", new_x=XPos.LMARGIN, new_y=YPos.NEXT)


def add_results_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "1. Resume Global")
    body_line(pdf, "Cette campagne confirme la stabilite de la version hebergee en production.")
    bullet(pdf, "URL Front: https://bi.tunibyte.com")
    bullet(pdf, "URL API: https://bi.tunibyte.com/api")
    bullet(pdf, "Couverture fonctionnelle: 14/14 PASS")
    bullet(pdf, "Taux de succes: 100%")
    pdf.ln(2)
    kpi_card_row(pdf)

    section_title(pdf, "2. Resultats Techniques")
    bullet(pdf, "xUnit backend: 12 PASS, 0 FAIL")
    bullet(pdf, "Selenium systeme: 2 PASS, 0 FAIL")
    bullet(pdf, "Checks API non fonctionnels: 2 PASS, 0 FAIL")
    bullet(pdf, "Security check: HTTP 401 sans token (attendu)")
    bullet(pdf, "Performance check: Average 66.7 ms, P95 81 ms")

    section_title(pdf, "3. Defaut Critique Traite")
    body_line(pdf, "BUG-AUTH-FIREBASE", bold=True)
    bullet(pdf, "Symptome: logout pendant filtrage client")
    bullet(pdf, "Cause: ecrasement JWT backend par token Firebase")
    bullet(pdf, "Correction: suppression du flux de remplacement token")
    bullet(pdf, "Verification: scenarios TC001, TC010, TC014 validant le correctif")


def add_architecture_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "2. Architecture Technique (Explication Complete)")
    body_line(pdf, "Objectif: expliquer clairement comment l'application est construite et ce qui a ete valide.")

    body_line(pdf, "Backend ASP.NET Core 8", bold=True)
    bullet(pdf, "Controllers: exposition des endpoints REST (Auth, Customers, Orders, Products, Analytics, Dashboard).")
    bullet(pdf, "Services: logique metier centrale (authentification, regles CRUD, aggregations KPIs).")
    bullet(pdf, "Repositories + UnitOfWork: acces donnees et transactions.")
    bullet(pdf, "Securite: JWT backend + roles, verification 401/403 sur endpoints proteges.")

    body_line(pdf, "Frontend Angular 17", bold=True)
    bullet(pdf, "Pages metier: login, dashboard, customers, produits, etc.")
    bullet(pdf, "Auth service: gestion session et stockage token backend.")
    bullet(pdf, "Guards + interceptors: protection routes et injection token dans appels API.")
    bullet(pdf, "UI testee via Selenium en conditions reelles sur domaine heberge.")

    body_line(pdf, "Infrastructure Hosted", bold=True)
    bullet(pdf, "Front: https://bi.tunibyte.com")
    bullet(pdf, "API: https://bi.tunibyte.com/api")
    bullet(pdf, "Nginx en reverse proxy + service backend sur VPS Ubuntu.")
    bullet(pdf, "Deploiement frontend valide avec bundle actif en production.")


def add_methodology_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "3. Strategie et Methodologie de Test")
    body_line(pdf, "La campagne suit une approche multi-niveaux pour maximiser la confiance produit.")

    body_line(pdf, "Niveaux de test", bold=True)
    bullet(pdf, "Unitaire: verification des branches critiques backend (12 tests xUnit).")
    bullet(pdf, "Integration: verification API, securite et performance en environnement heberge.")
    bullet(pdf, "Systeme: parcours utilisateur bout-en-bout via Selenium.")

    body_line(pdf, "Types de test", bold=True)
    bullet(pdf, "Fonctionnels: login, navigation, CRUD client, parcours critique.")
    bullet(pdf, "Non fonctionnels: securite acces non autorise, performance endpoints proteges.")
    bullet(pdf, "Regression: verification apres correction bug critique auth/session.")

    body_line(pdf, "Techniques de conception", bold=True)
    bullet(pdf, "Boite noire: classes d'equivalence, valeurs limites, scenarios utilisateur.")
    bullet(pdf, "Boite blanche: coverage de branches controlleurs/services.")
    bullet(pdf, "Traceabilite exigences -> cas de test -> evidence -> statut.")


def add_catalog_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "4. Catalogue des Cas de Test (14/14)")
    body_line(pdf, "Lecture rapide: chaque cas est aligne sur une exigence et execute en environnement heberge.")

    body_line(pdf, "Authentification et acces", bold=True)
    bullet(pdf, "TC001: Connexion valide Firebase + JWT local -> PASS")
    bullet(pdf, "TC002: Mot de passe incorrect -> PASS")
    bullet(pdf, "TC003: Endpoint admin sans token -> PASS (401/403)")

    body_line(pdf, "Donnees metier et analytics", bold=True)
    bullet(pdf, "TC004: Pagination clients limite basse -> PASS")
    bullet(pdf, "TC006: Recherche produit par terme -> PASS")
    bullet(pdf, "TC007: KPIs OLTP dashboard -> PASS")
    bullet(pdf, "TC008: Chargement dashboard DW UI -> PASS")
    bullet(pdf, "TC009: Performance endpoint protege -> PASS")

    body_line(pdf, "Regression et controles backend", bold=True)
    bullet(pdf, "TC010: Parcours critique global -> PASS")
    bullet(pdf, "TC011: Auth me avec claim invalide -> PASS")
    bullet(pdf, "TC012: Delete customer en echec service -> PASS")
    bullet(pdf, "TC013: Create user conflit metier -> PASS")
    bullet(pdf, "TC014: Ajout client E2E + verification + cleanup -> PASS")

    body_line(pdf, "Note", bold=True)
    bullet(pdf, "Resultat global: 14 PASS, 0 FAIL, 0 BLOQUE, 0 NON EXECUTE.")


def add_execution_detail_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "5. Detail d'Execution et Interpretation")

    body_line(pdf, "Selenium (tests systeme)", bold=True)
    bullet(pdf, "test_add_customer.py: login admin, add customer, recherche, suppression, verification finale.")
    bullet(pdf, "test_login_invalid_password.py: controle message d'erreur et non-redirection dashboard.")
    bullet(pdf, "Stabilisation appliquee: nettoyage session avant test login invalide pour eviter la flakiness.")

    body_line(pdf, "Checks API non fonctionnels", bold=True)
    bullet(pdf, "NF-SEC-01: tentative acces /users sans token -> HTTP 401 observe (conforme).")
    bullet(pdf, "NF-PERF-01: moyenne et P95 largement sous les seuils objectifs.")

    body_line(pdf, "Interpretation metier", bold=True)
    bullet(pdf, "Le socle d'authentification est stable apres correction du bug critique de token.")
    bullet(pdf, "Les parcours clients critiques sont operationnels en production hebergee.")
    bullet(pdf, "La plateforme est presentable en soutenance/demonstration avec risque maitrise.")


def add_incident_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "6. Incident Critique: Diagnostic et Resolution")

    body_line(pdf, "Symptome observe", bold=True)
    bullet(pdf, "Deconnexion utilisateur lors du filtrage clients (retour /login).")

    body_line(pdf, "Cause racine", bold=True)
    bullet(pdf, "Ecouteur Firebase (onAuthStateChanged) ecrasait le JWT backend en localStorage.")
    bullet(pdf, "Le token envoye aux endpoints proteges devenait invalide pour l'API backend.")

    body_line(pdf, "Correctif applique", bold=True)
    bullet(pdf, "Suppression du comportement d'ecrasement token dans AuthService frontend.")
    bullet(pdf, "Rebuild Angular + redeploiement VPS + rechargement Nginx.")

    body_line(pdf, "Validation post-correctif", bold=True)
    bullet(pdf, "Plus de redirection /login pendant les operations de filtrage/recherche.")
    bullet(pdf, "E2E add customer passe en continu sur URL hebergee.")
    bullet(pdf, "Statut final incident: FERME.")


def add_runbook_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "4. Runbook - Execution des Tests")

    body_line(pdf, "Etape 1 - Activation environnement Python", bold=True)
    command_block(
        pdf,
        "",
        [
            "cd d:\\mahdi-adem-v2",
            ".\\.venv\\Scripts\\Activate.ps1",
        ],
    )

    body_line(pdf, "Etape 2 - Suite Selenium sur URL hebergee", bold=True)
    command_block(
        pdf,
        "",
        [
            "cd d:\\mahdi-adem-v2\\BI\\test-qualite\\automation\\pytest-selenium",
            '$env:APP_BASE_URL = "https://bi.tunibyte.com"',
            '$env:TEST_USER_EMAIL = "admin@wideworldimporters.com"',
            '$env:TEST_USER_PASSWORD = "Admin@123"',
            '$env:HEADLESS = "true"  # false pour voir le navigateur',
            "d:/mahdi-adem-v2/.venv/Scripts/python.exe -m pytest -v --tb=short",
        ],
    )

    body_line(pdf, "Etape 3 - Checks API non fonctionnels", bold=True)
    command_block(
        pdf,
        "",
        [
            "cd d:\\mahdi-adem-v2\\BI\\test-qualite\\automation",
            "./nonfunctional_api_checks.ps1 -ApiBase \"https://bi.tunibyte.com/api\"",
        ],
    )


def add_artifacts_page(pdf: FPDF):
    pdf.add_page()
    pdf.set_y(18)
    section_title(pdf, "5. Pieces de Preuve")
    bullet(pdf, "test-qualite/resultats/pytest_report.html")
    bullet(pdf, "test-qualite/resultats/nonfunctional_api_checks.md")
    bullet(pdf, "test-qualite/resultats/nonfunctional_api_checks.csv")
    bullet(pdf, "test-qualite/resultats/dotnet_unit_tests.trx")
    bullet(pdf, "test-qualite/04_Rapport_Execution.md")
    bullet(pdf, "test-qualite/05_Rapport_Final.md")

    section_title(pdf, "6. Conclusion")
    body_line(pdf, "Version hebergee validee pour presentation et demonstration.", bold=True)
    bullet(pdf, "Stabilite fonctionnelle confirmee")
    bullet(pdf, "Performance et securite conformes aux seuils")
    bullet(pdf, "Risque residuel principal: tests unitaires frontend a activer")

    pdf.ln(6)


def main() -> None:
    pdf = PremiumPDF("P", "mm", "A4")
    pdf.set_auto_page_break(auto=True, margin=14)
    pdf.set_margins(12, 18, 12)

    add_cover(pdf)
    add_results_page(pdf)
    add_architecture_page(pdf)
    add_methodology_page(pdf)
    add_catalog_page(pdf)
    add_execution_detail_page(pdf)
    add_incident_page(pdf)
    add_runbook_page(pdf)
    add_artifacts_page(pdf)

    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    pdf.output(str(OUTPUT))


if __name__ == "__main__":
    main()
