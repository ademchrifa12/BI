from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC


class LoginPage:
    def __init__(self, driver, base_url: str):
        self.driver = driver
        self.base_url = base_url

    # Ajuster ces selecteurs selon le HTML reel du composant login
    email_input = (By.CSS_SELECTOR, 'input[formcontrolname="username"], input[type="email"], input[name="username"]')
    password_input = (By.CSS_SELECTOR, 'input[formcontrolname="password"], input[type="password"], input[name="password"]')
    submit_button = (By.CSS_SELECTOR, 'button[type="submit"], .login-btn')
    error_message = (By.CSS_SELECTOR, '.alert-error, .error-message, .alert-danger, .mat-mdc-snack-bar-label')

    def open(self):
        self.driver.get(f"{self.base_url}/login")
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located(self.submit_button)
        )

    def login(self, username: str, password: str):
        email = WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located(self.email_input)
        )
        pwd = self.driver.find_element(*self.password_input)
        btn = self.driver.find_element(*self.submit_button)

        email.clear()
        email.send_keys(username)
        pwd.clear()
        pwd.send_keys(password)
        btn.click()

    def login_and_wait_success(self, username: str, password: str):
        self.login(username, password)

        WebDriverWait(self.driver, 20).until(
            lambda d: "/login" not in d.current_url
        )

    def login_and_wait_error(self, username: str, password: str):
        self.login(username, password)
        return self.get_error_text()

    def get_error_text(self):
        err = WebDriverWait(self.driver, 15).until(
            EC.visibility_of_element_located(self.error_message)
        )
        return err.text.strip()
