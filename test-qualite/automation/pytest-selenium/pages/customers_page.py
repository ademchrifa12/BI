from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC


class CustomersPage:
    def __init__(self, driver, base_url: str):
        self.driver = driver
        self.base_url = base_url

    add_customer_button = (By.XPATH, "//button[contains(normalize-space(.), 'Add Customer')]")
    search_input = (By.CSS_SELECTOR, "input[placeholder='Search customers...']")
    table_rows = (By.CSS_SELECTOR, "table.data-table tbody tr")

    customer_name_input = (By.CSS_SELECTOR, ".modal .modal-body input[placeholder='Enter customer name']")
    phone_input = (By.CSS_SELECTOR, ".modal .modal-body input[placeholder='Enter phone']")
    email_input = (By.CSS_SELECTOR, ".modal .modal-body input[placeholder='Enter email']")

    create_button = (By.XPATH, "//div[contains(@class,'modal-footer')]//button[contains(normalize-space(.), 'Create')]")
    confirm_delete_button = (By.XPATH, "//div[contains(@class,'modal-footer')]//button[contains(normalize-space(.), 'Delete')]")

    def open(self):
        self.driver.get(f"{self.base_url}/customers")
        WebDriverWait(self.driver, 30).until(
            lambda d: "/login" in d.current_url or len(d.find_elements(*self.search_input)) > 0
        )

        if "/login" in self.driver.current_url:
            raise AssertionError(
                f"Redirected to login while opening customers page. Current URL: {self.driver.current_url}"
            )

    def can_add_customer(self) -> bool:
        return len(self.driver.find_elements(*self.add_customer_button)) > 0

    def open_add_modal(self):
        btn = WebDriverWait(self.driver, 10).until(
            EC.element_to_be_clickable(self.add_customer_button)
        )
        btn.click()
        WebDriverWait(self.driver, 10).until(
            EC.visibility_of_element_located(self.customer_name_input)
        )

    def create_customer(self, customer_name: str, phone: str = "", email: str = ""):
        name = self.driver.find_element(*self.customer_name_input)
        name.clear()
        name.send_keys(customer_name)

        if phone:
            phone_input = self.driver.find_element(*self.phone_input)
            phone_input.clear()
            phone_input.send_keys(phone)

        if email:
            email_input = self.driver.find_element(*self.email_input)
            email_input.clear()
            email_input.send_keys(email)

        self.driver.find_element(*self.create_button).click()

        WebDriverWait(self.driver, 20).until(
            EC.invisibility_of_element_located(self.customer_name_input)
        )

    def search(self, value: str):
        WebDriverWait(self.driver, 20).until(
            lambda d: "/login" in d.current_url or len(d.find_elements(*self.search_input)) > 0
        )

        if "/login" in self.driver.current_url:
            raise AssertionError(
                f"Redirected to login before search. Current URL: {self.driver.current_url}"
            )

        search = self.driver.find_element(*self.search_input)
        search.clear()
        search.send_keys(value)

        # Wait for debounce (300 ms) to fire + API response to update the table.
        # The table rows still contain old data right after typing, so we must
        # wait until the rendered rows actually reflect the search term.
        WebDriverWait(self.driver, 15).until(
            lambda d: self._search_result_loaded(d, value)
        )

    def _search_result_loaded(self, d, value: str) -> bool:
        """Return True once the table shows results filtered by *value*."""
        rows = d.find_elements(By.CSS_SELECTOR, "table.data-table tbody tr")
        for row in rows:
            text = row.text.strip().lower()
            if "no customers found" in text:
                return True  # search completed, no matches
            if value.lower() in text:
                return True  # found a matching row
        return False

    def is_customer_present(self, customer_name: str) -> bool:
        rows = self.driver.find_elements(*self.table_rows)
        for row in rows:
            text = row.text.strip().lower()
            if customer_name.lower() in text and "no customers found" not in text:
                return True
        return False

    def delete_customer_if_present(self, customer_name: str) -> bool:
        rows = self.driver.find_elements(*self.table_rows)
        for row in rows:
            text = row.text.strip().lower()
            if customer_name.lower() not in text:
                continue

            delete_buttons = row.find_elements(By.CSS_SELECTOR, "button.btn-icon.danger")
            if not delete_buttons:
                return False

            delete_buttons[0].click()
            WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable(self.confirm_delete_button)
            ).click()

            WebDriverWait(self.driver, 15).until(
                lambda d: not self.is_customer_present(customer_name)
            )
            return True

        return False
