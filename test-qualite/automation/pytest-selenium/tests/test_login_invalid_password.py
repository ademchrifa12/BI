from pages.login_page import LoginPage


def test_login_invalid_password_shows_error(browser, base_url):
    # TC002 - Systeme/Fonctionnel
    page = LoginPage(browser, base_url)
    page.open()

    page.login("admin@wideworldimporters.com", "WrongPassword123!")

    current_url = browser.current_url
    assert "/login" in current_url, "L'utilisateur ne doit pas etre redirige vers dashboard"

    error_text = page.get_error_text().lower()
    expected_keywords = ["invalid", "incorrect", "erreur", "mot de passe"]
    assert any(k in error_text for k in expected_keywords), (
        f"Message d'erreur inattendu: {error_text}"
    )
