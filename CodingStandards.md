# 🧑‍💻 Coding Standards

> **Note:** More coding conventions will be added after meeting with the client.  
> This includes decisions related to the tech stack, variable/method naming conventions, patterns, folder structure, languages, and frameworks.

---

## 1. Branch Naming Conventions
- Branch names must follow this format:
FirstName-Issue#
- **Example:** `Jordan-25`

---

## 2. Interfaces
- All interfaces must begin with a capital **`I`**
- **Example:** `IUserService`

---

## 3. Database Standards
- Database table names must be **plural**
- **Examples:** `Users`, `Orders`, `Comments`

---

## 4. Separation of Concerns
- ❌ **NO SQL QUERIES IN UI VIEWS**
- UI layers must not contain:
- Database logic
- Raw SQL queries
- Database operations must be handled through proper **data access** or **service layers**

---

## 5. Comments & Documentation Standards
- Add comments to complex or non-obvious code
- Use summary tags to explain the intent of methods at a high level
- Avoid commenting obvious code
- Comments should explain **why** something is done, not just **what** it does

---

## 6. File & Code Organization
- Keep code modular and well-organized
- Avoid large files with unrelated responsibilities
- Each file should focus on a single purpose

---

## 7. Method Quality Standards
- Methods should follow the **Single Responsibility Principle**
- Avoid excessively long methods; refactor when necessary
- Reduce deep nesting using early returns when appropriate
- Do not combine UI, validation, business logic, and database access in a single method

---

## 8. Input Validation Rules
- Validate all user input before processing or saving data
- Do not rely solely on client-side validation
- Validation should be consistent and not unnecessarily duplicated

---

## 9. Error Handling Rules
- Do not use empty catch blocks
- Do not silently ignore errors
- Handle errors to prevent crashes when possible
- Provide meaningful error messages, especially for user-facing actions

---

## 10. Logging Standards
- Log errors and important events with enough detail for debugging
- Do not log sensitive information such as:
- Passwords
- Tokens
- Private user data

---

## 11. Security Standards
- Never hardcode sensitive information, including:
- Passwords
- API keys
- Access tokens
- Connection strings
- Store sensitive values securely using configuration or environment variables

---

## 12. Version Control (Git) Standards
- Commit frequently with clear, descriptive messages
- Avoid vague commit messages (e.g., `fix`, `stuff`, `updates`, `final`)
- Commits should represent logical, focused units of work

---

## 13. Merge & Collaboration Rules
- Do not push unfinished or broken code to shared branches
- Resolve merge conflicts carefully
- Verify the project builds and runs after merging
- Always test before merging

---

## 14. Testing & Quality Assurance
- Test core functionality after changes
- Verify edge cases and error scenarios when possible
- Fix bugs before adding new features when reasonable

---

## 15. Performance & Efficiency Standards
- Avoid repeated expensive operations (e.g., database calls inside loops)
- Avoid unnecessary duplicate queries or redundant processing
- Write reasonably efficient code by default; optimize when necessary
