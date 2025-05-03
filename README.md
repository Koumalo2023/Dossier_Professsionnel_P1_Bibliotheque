# Projet Bibliothèque : API et Client
Ce projet est une application de gestion de bibliothèque divisée en deux composants principaux :

1- API Backend : Construite avec ASP.NET 8 et utilisant SQL Server comme base de données.
2- Client Frontend : Construit avec Angular 18 pour fournir une interface utilisateur moderne.

## Configuration de l'API Backend

# Prérequis API
    .NET 8 SDK
    *SQL Server (ou SQL Server Express)
    Visual Studio 2022+ (optionnel, mais recommandé pour le développement .NET)
    Postman ou Swagger pour tester les endpoints.

# Installation API

1. Clonez le dépôt : 
2. Configurez la chaîne de connexion SQL Server dans appsettings.json :
3. Installez les dépendances NuGet
4. Appliquez les migrations pour créer la base de données :


# Exécution API
1. Lancez l'application en mode développement :
2. L'API sera accessible à l'adresse suivante :
3. Vous pouvez accéder à Swagger pour tester les endpoints :

Base de données
La base de données utilise SQL Server. Les migrations Entity Framework Core sont utilisées pour gérer le schéma de la base de données. Assurez-vous que SQL Server est configuré correctement et que les rôles (Admin, User) sont initialisés lors du démarrage de l'application.


## Configuration du Client Frontend

# Prérequis Client
1. Node.js 20+
2. Angular CLI 18
3. Navigateur web moderne (Chrome, Firefox, Edge).

# Installation Client

1. Clonez le dépôt (si ce n'est pas déjà fait) :
2. Installez les dépendances npm:
3. Configurez l'URL de l'API dans src/environments/environment.ts :
4. 

# Exécution Client
1. Lancez l'application Angular en mode développement :
2. L'application sera accessible à l'adresse suivante :

## Structure du projet

# API Backend
/api
    ├── Controllers/
    │   ├── AuthController.cs      
    │   ├── UsersController.cs      
    │   ├── BooksController.cs      
    │   ├── LoansController.cs      
    │   ├── StatisticsController.cs 
    │   └── NotificationsController.cs 
    ├── Models/
    │   ├── User.cs                 
    │   ├── Book.cs                 
    │   ├── Loan.cs                 
    │   └── Notification.cs         
    ├── DTOs/
    │   ├── UserDto.cs              
    │   ├── BookDto.cs              
    │   ├── LoanDto.cs              
    │   └── NotificationDto.cs      
    ├── Services/
    │   ├── AuthService.cs          
    │   ├── UserService.cs          
    │   ├── BookService.cs          
    │   ├── LoanService.cs          
    │   ├── StatisticsService.cs    
    │   └── NotificationService.cs 
    ├── Repositories/
    │   ├── IUserRepository.cs      
    │   ├── UserRepository.cs       
    │   ├── IBookRepository.cs      
    │   ├── BookRepository.cs       
    │   ├── ILoanRepository.cs      
    │   ├── LoanRepository.cs       
    │   └── INotificationRepository.cs 
    │   └── NotificationRepository.cs  
    ├── Migrations/                 
    ├── Helpers/
    │   ├── JwtHelper.cs           
    │   └── PaginationHelper.cs     
    ├── AppDbContext.cs             
    ├── Program.cs                  
    └── appsettings.json            

# Client Frontend

 /client
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── guards/
│   │   │   ├── interceptors/
│   │   │   ├── models/
│   │   │   └── services/
│   │   │       ├── auth.service.ts
│   │   │       ├── book.service.ts
│   │   │       ├── loan.service.ts
│   │   │       ├── notification.service.ts
│   │   │       ├── statistics.service.ts
│   │   │       └── user.service.ts
│   │   ├── features/
│   │   │   ├── admin/
│   │   │   ├── auth/
│   │   │   │   ├── auth.component.ts
│   │   │   │   └── auth-routing.module.ts
│   │   │   ├── public/
│   │   │   ├── user/
│   │   │   │   ├── users/
│   │   │   │   │   ├── profile.component.ts
│   │   │   │   │   ├── user-list.component.ts
│   │   │   │   │   └── users-routing.module.ts
│   │   │   │   ├── loans/
│   │   │   │   │   ├── loan-list.component.ts
│   │   │   │   │   ├── my-loans.component.ts
│   │   │   │   │   └── loans-routing.module.ts
│   │   │   │   ├── books/
│   │   │   │   │   ├── book-list.component.ts
│   │   │   │   │   ├── book-detail.component.ts
│   │   │   │   │   ├── book-form.component.ts
│   │   │   │   │   └── books-routing.module.ts
│   │   │   │   ├── statistics/
│   │   │   │   │   ├── overview.component.ts
│   │   │   │   │   ├── top-books.component.ts
│   │   │   │   │   ├── user-history.component.ts
│   │   │   │   │   └── statistics-routing.module.ts
│   │   │   │   ├── notifications/
│   │   │   │   │   ├── notification-list.component.ts
│   │   │   │   │   └── notifications-routing.module.ts
│   │   ├── shared/
│   │   │   ├── components/
│   │   ├── app-routing.module.ts
│   │   ├── app.component.ts
│   │   └── app.module.ts
│   ├── assets/
│   ├── environments/
│   └── main.ts
└── angular.json               


## Contributions

Nous encourageons les contributions ! Si vous souhaitez contribuer au projet :

1. Fork le dépôt.
2. Créez une branche pour vos modifications :
    git checkout -b feature/nom-de-la-fonctionnalite

3. Soumettez une pull request avec une description claire de vos changements.

## Licence
Ce projet est sous licence MIT. Consultez le fichier LICENSE pour plus de détails.
