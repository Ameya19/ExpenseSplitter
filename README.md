# 💸 ExpenseSplitter

A full-stack web application for splitting shared expenses among groups of people. Built with **ASP.NET Core 9 Web API** and **Angular 17**, it simplifies tracking shared costs and settling debts within groups.

---

## ✨ Features

- 🔐 **Authentication** — Register, login with JWT-based authentication
- 👥 **Groups** — Create groups, invite members, manage roles (Admin/Member)
- 💸 **Expenses** — Add shared expenses with Equal, Percentage, or Exact split types
- ⚖️ **Balances** — Automatic balance calculation showing who owes whom
- 🤝 **Settlements** — Settle debts with minimum transactions using greedy algorithm
- 🔔 **Notifications** — Real-time notifications for group activity
- 📊 **Reports** — Monthly spending breakdown with category analysis
- 👤 **Profile** — Manage your account and change password

---

## 🛠️ Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 9 | Web API framework |
| Entity Framework Core | ORM |
| PostgreSQL | Database |
| ASP.NET Identity | Authentication & user management |
| JWT Bearer | Token-based authorization |
| Clean Architecture | Project structure |

### Frontend
| Technology | Purpose |
|---|---|
| Angular 17 | Frontend framework |
| Angular Material | UI component library |
| TypeScript | Programming language |
| SCSS | Styling |
| Reactive Forms | Form handling |
| Angular Guards | Route protection |
| HTTP Interceptors | JWT token attachment |

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- PostgreSQL 14+
- Angular CLI 17+

---

### Backend Setup

**1. Clone the repository**
```bash
git clone https://github.com/yourusername/expense-splitter.git
cd expense-splitter
```

**2. Configure database connection**

Update `appsettings.Development.json` in `ExpenseSplitter.API`:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=ExpenseSplitterDb;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-minimum-32-characters",
    "Issuer": "ExpenseSplitter",
    "Audience": "ExpenseSplitter"
  }
}
```

**3. Run database migrations**
```bash
cd ExpenseSplitter.API
dotnet ef database update \
  --project ../ExpenseSplitter.Infrastructure
```

**4. Run the API**
```bash
dotnet run
```

API runs at `https://localhost:7194`
Swagger UI at `https://localhost:7194/swagger`

---

### Frontend Setup

**1. Install dependencies**
```bash
cd expense-splitter-ui
npm install
```

**2. Configure environment**

Update `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7194/api'
};
```

**3. Run the app**
```bash
ng serve
```

App runs at `http://localhost:4200`

---

## 💡 Key Algorithms

### Debt Simplification
Uses a **greedy algorithm** to minimize the number of transactions needed to settle all debts within a group:
Example — 3 people:
Alice  +₹1500  (is owed)
Bob    +₹500   (is owed)
Carol  -₹2000  (owes)
Result — only 2 transactions:
✅ Carol pays Alice ₹1500
✅ Carol pays Bob   ₹500

### Balance Calculation
Balances are calculated dynamically from expenses and splits — no separate balance table needed:
Net Balance = Total Paid - Total Owed + Settlements

---

## 🔒 Security

- JWT tokens with configurable expiry
- Password hashing via ASP.NET Identity
- Role-based access (Admin/Member) per group
- Protected routes with Angular Guards
- JWT interceptor for automatic token attachment

---

## 👨‍💻 Author

**Ameya**
- GitHub: [@Ameya19](https://github.com/Ameya19)
- LinkedIn: [linkedin.com/in/ameya19](https://linkedin.com/in/ameya19)
