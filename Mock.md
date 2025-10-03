# Données Mock pour l'API Bibliothèque

Ce fichier contient des données de test au format JSON pour tester les endpoints de l'API de bibliothèque.

## Notes Importantes sur la Liaison Livres-Catégories

**IMPORTANT :** L'API gère automatiquement la liaison entre les livres et les catégories. Lors de la création ou modification d'un livre :
- Le champ `category` attend le **nom de la catégorie** (string)
- Si la catégorie n'existe pas, elle est **créée automatiquement**
- La liaison CategoryId est gérée automatiquement par le service

## Endpoints d'Authentification

### Connexion (Login)

**Endpoint:** `POST /api/auth/login`

```json
{
  "email": "admin@bibliotheque.fr",
  "password": "Admin123!"
}
```

```json
{
  "email": "user@bibliotheque.fr", 
  "password": "User123!"
}
```

```json
{
  "email": "jean.dupont@email.com",
  "password": "Password123"
}
```

### Inscription (Register)

**Endpoint:** `POST /api/auth/register`

```json
{
  "name": "Marie Curie",
  "email": "marie.curie@email.com",
  "password": "Marie123!"
}
```

```json
{
  "name": "Albert Einstein",
  "email": "albert.einstein@email.com", 
  "password": "Einstein123!"
}
```

```json
{
  "name": "Ada Lovelace",
  "email": "ada.lovelace@email.com",
  "password": "Ada123!"
}
```

## Endpoints des Catégories

### Créer une catégorie

**Endpoint:** `POST /api/books/categories`

```json
{
  "name": "Science-Fiction",
  "description": "Livres de science-fiction et d'anticipation",
  "icon": "🚀"
}
```

```json
{
  "name": "Fantasy",
  "description": "Livres de fantasy et de magie",
  "icon": "🧙"
}
```

```json
{
  "name": "Littérature Classique",
  "description": "Œuvres littéraires classiques",
  "icon": "📚"
}
```

```json
{
  "name": "Policier",
  "description": "Romans policiers et thrillers",
  "icon": "🔍"
}
```

```json
{
  "name": "Jeunesse",
  "description": "Livres pour enfants et adolescents",
  "icon": "👦"
}
```

## Endpoints des Livres

### Créer un livre

**Endpoint:** `POST /api/books`

```json
{
  "title": "Le Petit Prince",
  "author": "Antoine de Saint-Exupéry",
  "category": "Littérature Classique",
  "isbn": "978-2-07-040000-0",
  "publicationDate": "1943-04-06T00:00:00Z",
  "coverUrl": "https://example.com/covers/le-petit-prince.jpg",
  "totalCopies": 5
}
```

```json
{
  "title": "1984",
  "author": "George Orwell",
  "category": "Science-Fiction",
  "isbn": "978-2-07-036000-1",
  "publicationDate": "1949-06-08T00:00:00Z",
  "coverUrl": "https://example.com/covers/1984.jpg",
  "totalCopies": 3
}
```

```json
{
  "title": "Harry Potter à l'École des Sorciers",
  "author": "J.K. Rowling",
  "category": "Fantasy",
  "isbn": "978-2-07-054120-2",
  "publicationDate": "1997-06-26T00:00:00Z",
  "coverUrl": "https://example.com/covers/harry-potter.jpg",
  "totalCopies": 8
}
```

```json
{
  "title": "Fondation",
  "author": "Isaac Asimov",
  "category": "Science-Fiction",
  "isbn": "978-2-266-00000-3",
  "publicationDate": "1951-01-01T00:00:00Z",
  "coverUrl": "https://example.com/covers/fondation.jpg",
  "totalCopies": 4
}
```

```json
{
  "title": "Dune",
  "author": "Frank Herbert",
  "category": "Science-Fiction",
  "isbn": "978-2-290-00000-1",
  "publicationDate": "1965-08-01T00:00:00Z",
  "coverUrl": "https://example.com/covers/dune.jpg",
  "totalCopies": 6
}
```

### Mettre à jour un livre

**Endpoint:** `PUT /api/books/{id}`

```json
{
  "title": "Le Petit Prince - Édition Spéciale",
  "author": "Antoine de Saint-Exupéry",
  "category": "Littérature Classique",
  "isbn": "978-2-07-040000-0",
  "publicationDate": "1943-04-06T00:00:00Z",
  "coverUrl": "https://example.com/covers/le-petit-prince-special.jpg",
  "totalCopies": 7
}
```

```json
{
  "title": "1984 - Édition Anniversaire",
  "author": "George Orwell",
  "category": "Science-Fiction Dystopique",
  "isbn": "978-2-07-036000-1",
  "publicationDate": "1949-06-08T00:00:00Z",
  "coverUrl": "https://example.com/covers/1984-anniversaire.jpg",
  "totalCopies": 5
}
```

## Endpoints des Emprunts

### Créer un emprunt

**Endpoint:** `POST /api/loans`

```json
{
  "bookId": "12345678-1234-1234-1234-123456789abc",
  "dueDate": "2024-02-15T00:00:00Z"
}
```

```json
{
  "bookId": "abcdef12-3456-7890-abcd-ef1234567890",
  "dueDate": "2024-02-28T00:00:00Z"
}
```

```json
{
  "bookId": "11111111-2222-3333-4444-555555555555",
  "dueDate": "2024-03-10T00:00:00Z"
}
```

## Endpoints des Réservations

### Créer une réservation

**Endpoint:** `POST /api/reservations`

```json
{
  "bookId": "12345678-1234-1234-1234-123456789abc"
}
```

```json
{
  "bookId": "abcdef12-3456-7890-abcd-ef1234567890"
}
```

```json
{
  "bookId": "11111111-2222-3333-4444-555555555555"
}
```

## Endpoints des Notifications

### Créer une notification

**Endpoint:** `POST /api/notifications`

```json
{
  "userId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "message": "Votre livre 'Le Petit Prince' est disponible pour retrait",
  "type": "INFO"
}
```

```json
{
  "userId": "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
  "message": "Rappel : votre emprunt arrive à échéance dans 3 jours",
  "type": "WARNING"
}
```

```json
{
  "userId": "cccccccc-dddd-eeee-ffff-gggggggggggg",
  "message": "Votre réservation a été annulée",
  "type": "ERROR"
}
```

## Endpoints des Analytics

### Créer un log analytique

**Endpoint:** `POST /api/analytics`

```json
{
  "metricType": "MostBorrowed",
  "value": 15,
  "referenceDate": "2024-01-01T00:00:00Z",
  "referenceId": "12345678-1234-1234-1234-123456789abc"
}
```

```json
{
  "metricType": "ActiveUsers",
  "value": 42,
  "referenceDate": "2024-01-01T00:00:00Z"
}
```

```json
{
  "metricType": "OverdueLoans",
  "value": 3,
  "referenceDate": "2024-01-01T00:00:00Z"
}
```

```json
{
  "metricType": "NewRegistrations",
  "value": 8,
  "referenceDate": "2024-01-01T00:00:00Z"
}
```

## Endpoints d'Audit

### Créer un log d'audit

**Endpoint:** `POST /api/audit`

```json
{
  "userId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "userName": "Jean Dupont",
  "action": "CREATE_BOOK",
  "entityName": "Book",
  "entityId": "12345678-1234-1234-1234-123456789abc",
  "details": "Création du livre 'Le Petit Prince'",
  "ipAddress": "192.168.1.100"
}
```

```json
{
  "userId": "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
  "userName": "Marie Curie",
  "action": "UPDATE_BOOK",
  "entityName": "Book",
  "entityId": "abcdef12-3456-7890-abcd-ef1234567890",
  "details": "Mise à jour du nombre de copies",
  "ipAddress": "192.168.1.101"
}
```

```json
{
  "userId": null,
  "userName": "System",
  "action": "AUTO_BACKUP",
  "entityName": "Database",
  "entityId": null,
  "details": "Sauvegarde automatique de la base de données",
  "ipAddress": "127.0.0.1"
}
```

## Données de Test Réutilisables

### IDs de Référence

```json
{
  "bookIds": [
    "12345678-1234-1234-1234-123456789abc",
    "abcdef12-3456-7890-abcd-ef1234567890", 
    "11111111-2222-3333-4444-555555555555",
    "22222222-3333-4444-5555-666666666666",
    "33333333-4444-5555-6666-777777777777"
  ],
  "userIds": [
    "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
    "cccccccc-dddd-eeee-ffff-gggggggggggg",
    "dddddddd-eeee-ffff-gggg-hhhhhhhhhhhh"
  ],
  "categoryNames": [
    "Science-Fiction",
    "Fantasy",
    "Littérature Classique",
    "Policier",
    "Jeunesse",
    "Science-Fiction Dystopique"
  ]
}
```

### Dates de Référence

```json
{
  "currentDate": "2024-01-15T00:00:00Z",
  "nextWeek": "2024-01-22T00:00:00Z",
  "nextMonth": "2024-02-15T00:00:00Z",
  "lastMonth": "2023-12-15T00:00:00Z"
}
```

## Exemples de Requêtes HTTP Complètes

### Test d'Authentification

```http
POST http://localhost:5265/api/auth/login
Content-Type: application/json

{
  "email": "admin@bibliotheque.fr",
  "password": "Admin123!"
}
```

### Test de Création de Catégorie

```http
POST http://localhost:5265/api/books/categories
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Science-Fiction",
  "description": "Livres de science-fiction et d'anticipation",
  "icon": "🚀"
}
```

### Test de Création de Livre

```http
POST http://localhost:5265/api/books
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Dune",
  "author": "Frank Herbert",
  "category": "Science-Fiction",
  "isbn": "978-2-290-00000-1",
  "publicationDate": "1965-08-01T00:00:00Z",
  "coverUrl": "https://example.com/covers/dune.jpg",
  "totalCopies": 6
}
```

### Test de Création d'Emprunt

```http
POST http://localhost:5265/api/loans
Content-Type: application/json
Authorization: Bearer {token}

{
  "bookId": "12345678-1234-1234-1234-123456789abc",
  "dueDate": "2024-02-20T00:00:00Z"
}
```

## Ordre Recommandé pour les Tests

1. **Authentification** : Obtenir un token JWT
2. **Catégories** : Créer les catégories nécessaires (optionnel - l'API les crée automatiquement)
3. **Livres** : Créer des livres avec les noms de catégories
4. **Emprunts/Réservations** : Tester les fonctionnalités d'emprunt
5. **Notifications/Analytics/Audit** : Tester les fonctionnalités avancées

## Notes d'Utilisation

1. **Tokens d'authentification** : Les endpoints protégés nécessitent un token JWT dans l'en-tête Authorization
2. **Gestion automatique des catégories** : L'API crée automatiquement les catégories si elles n'existent pas
3. **Validation** : Toutes les données respectent les contraintes de validation définies dans les DTOs
4. **IDs de référence** : Utilisez les IDs fournis pour une cohérence dans les tests

Ces données couvrent l'ensemble des endpoints de l'API et peuvent être utilisées pour tester les fonctionnalités principales de la bibliothèque avec la liaison correcte entre livres et catégories.