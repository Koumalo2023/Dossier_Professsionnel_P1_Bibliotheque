import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from '../../../core/services/api/book.service';
import { LoanService } from '../../../core/services/api/loan.service';
import { ReservationService } from '../../../core/services/api/reservation.service';

interface Book {
  id: string;
  title: string;
  author: string;
  isbn: string;
  category: string;
  status: 'available' | 'borrowed' | 'reserved';
  publicationDate: string;
  publisher: string;
  description: string;
  coverImage?: string;
  totalCopies: number;
  availableCopies: number;
  pages?: number;
  language?: string;
  rating?: number;
  reviews?: Review[];
  similarBooks?: Book[];
}

interface Review {
  id: string;
  userId: string;
  userName: string;
  rating: number;
  comment: string;
  date: string;
}

interface LoanHistory {
  id: string;
  userId: string;
  userName: string;
  loanDate: string;
  returnDate?: string;
  status: 'active' | 'returned' | 'overdue';
}

@Component({
  selector: 'app-book-details-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './book-details-page.component.html'
})
export class BookDetailsPageComponent implements OnInit {
  book: Book | null = null;
  loanHistory: LoanHistory[] = [];
  similarBooks: Book[] = [];
  
  // États
  isLoading: boolean = false;
  hasError: boolean = false;
  isReserving: boolean = false;
  isBorrowing: boolean = false;
  
  // Actions utilisateur
  showFullDescription: boolean = false;
  selectedTab: string = 'details';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private loanService: LoanService,
    private reservationService: ReservationService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const bookId = params.get('id');
      if (bookId) {
        this.loadBookDetails(bookId);
      } else {
        this.hasError = true;
      }
    });
  }

  loadBookDetails(bookId: string): void {
    this.isLoading = true;
    this.hasError = false;

    // Simulation de données - à remplacer par des appels API réels
    setTimeout(() => {
      try {
        this.generateMockBookData(bookId);
        this.isLoading = false;
      } catch (error) {
        this.hasError = true;
        this.isLoading = false;
        console.error('Erreur lors du chargement des détails du livre:', error);
      }
    }, 1000);
  }

  private generateMockBookData(bookId: string): void {
    // Données du livre principal
    this.book = {
      id: bookId,
      title: 'Le Petit Prince',
      author: 'Antoine de Saint-Exupéry',
      isbn: '978-2-07-040000-0',
      category: 'Fiction',
      status: 'available',
      publicationDate: '1943-04-06',
      publisher: 'Gallimard',
      description: `Un conte poétique et philosophique sous l'apparence d'un conte pour enfants. 
      
"Le Petit Prince" est une œuvre de langue française, la plus connue d'Antoine de Saint-Exupéry. Publié en 1943 à New York simultanément à sa traduction anglaise, c'est une œuvre poétique et philosophique sous l'apparence d'un conte pour enfants.

Traduit en trois cent soixante et une langues, "Le Petit Prince" est le deuxième ouvrage le plus traduit au monde après la Bible. Le livre est dédié à Léon Werth, mais « quand il était petit garçon ».
      
Le narrateur est un aviateur qui, à la suite d'une panne de moteur, a dû se poser en catastrophe dans le désert du Sahara et tente de réparer son avion. Le lendemain de son atterrissage forcé, il est réveillé par une petite voix qui lui demande : « S'il vous plaît… dessine-moi un mouton ! »`,
      coverImage: 'https://via.placeholder.com/300x400?text=Le+Petit+Prince',
      totalCopies: 5,
      availableCopies: 3,
      pages: 96,
      language: 'Français',
      rating: 4.8,
      reviews: [
        {
          id: '1',
          userId: 'user1',
          userName: 'Marie Dupont',
          rating: 5,
          comment: 'Un chef-d\'œuvre intemporel ! La profondeur philosophique de ce livre est incroyable.',
          date: '2024-01-15'
        },
        {
          id: '2',
          userId: 'user2',
          userName: 'Jean Martin',
          rating: 4,
          comment: 'Belle histoire, même pour les adultes. Les illustrations sont magnifiques.',
          date: '2024-01-10'
        },
        {
          id: '3',
          userId: 'user3',
          userName: 'Sophie Leroy',
          rating: 5,
          comment: 'Je relis ce livre chaque année. Il me rappelle l\'essentiel de la vie.',
          date: '2024-01-05'
        }
      ]
    };

    // Historique des prêts
    this.loanHistory = [
      {
        id: 'loan1',
        userId: 'user4',
        userName: 'Pierre Bernard',
        loanDate: '2024-01-20',
        returnDate: '2024-02-10',
        status: 'returned'
      },
      {
        id: 'loan2',
        userId: 'user5',
        userName: 'Claire Moreau',
        loanDate: '2024-02-15',
        returnDate: '2024-03-05',
        status: 'returned'
      },
      {
        id: 'loan3',
        userId: 'user6',
        userName: 'Thomas Petit',
        loanDate: '2024-03-10',
        returnDate: undefined,
        status: 'active'
      }
    ];

    // Livres similaires
    this.similarBooks = [
      {
        id: '2',
        title: 'L\'Étranger',
        author: 'Albert Camus',
        isbn: '978-2-07-036002-0',
        category: 'Philosophie',
        status: 'available',
        publicationDate: '1942-01-01',
        publisher: 'Gallimard',
        description: 'Roman existentialiste sur l\'absurdité de la condition humaine.',
        coverImage: 'https://via.placeholder.com/150x200?text=L+Etranger',
        totalCopies: 4,
        availableCopies: 2
      },
      {
        id: '3',
        title: 'La Peste',
        author: 'Albert Camus',
        isbn: '978-2-07-036003-0',
        category: 'Philosophie',
        status: 'available',
        publicationDate: '1947-01-01',
        publisher: 'Gallimard',
        description: 'Roman sur une épidémie de peste dans la ville d\'Oran.',
        coverImage: 'https://via.placeholder.com/150x200?text=La+Peste',
        totalCopies: 3,
        availableCopies: 1
      },
      {
        id: '4',
        title: 'Vol de nuit',
        author: 'Antoine de Saint-Exupéry',
        isbn: '978-2-07-036004-0',
        category: 'Fiction',
        status: 'borrowed',
        publicationDate: '1931-01-01',
        publisher: 'Gallimard',
        description: 'Roman sur les pionniers de l\'aviation postale.',
        coverImage: 'https://via.placeholder.com/150x200?text=Vol+de+nuit',
        totalCopies: 2,
        availableCopies: 0
      }
    ];
  }

  onReserveBook(): void {
    if (!this.book) return;
    
    this.isReserving = true;
    // Simulation de réservation
    setTimeout(() => {
      this.isReserving = false;
      alert(`Livre "${this.book?.title}" réservé avec succès !`);
    }, 1000);
  }

  onBorrowBook(): void {
    if (!this.book) return;
    
    this.isBorrowing = true;
    // Simulation d'emprunt
    setTimeout(() => {
      this.isBorrowing = false;
      alert(`Livre "${this.book?.title}" emprunté avec succès !`);
    }, 1000);
  }

  onTabChange(tab: string): void {
    this.selectedTab = tab;
  }

  toggleDescription(): void {
    this.showFullDescription = !this.showFullDescription;
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'available': return 'Disponible';
      case 'borrowed': return 'Emprunté';
      case 'reserved': return 'Réservé';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'available': return 'status-available';
      case 'borrowed': return 'status-borrowed';
      case 'reserved': return 'status-reserved';
      default: return 'status-unknown';
    }
  }

  getAvailabilityText(): string {
    if (!this.book) return '';
    
    if (this.book.availableCopies === 0) {
      return 'Aucun exemplaire disponible';
    } else if (this.book.availableCopies === 1) {
      return '1 exemplaire disponible';
    } else {
      return `${this.book.availableCopies} exemplaires disponibles`;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR');
  }

  navigateToBook(bookId: string): void {
    this.router.navigate(['/books', bookId]);
  }

  getStarRating(rating: number): string[] {
    const stars = [];
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;

    for (let i = 0; i < fullStars; i++) {
      stars.push('★');
    }
    if (hasHalfStar) {
      stars.push('½');
    }
    while (stars.length < 5) {
      stars.push('☆');
    }

    return stars;
  }
}
