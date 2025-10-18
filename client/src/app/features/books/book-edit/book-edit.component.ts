import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';

import { BookService } from '../../../core/services/api/book.service';
import { Book, CreateBookRequest, UpdateBookRequest, Category, GoogleBookPreview, ImportBookFromGoogleRequest } from '../../../core/models/book.model';
import { ToastService } from '../../../core/services/toast.service';

import { ButtonComponent } from '../../../shared/components/atoms/button/button.component';
import { IconComponent } from '../../../shared/components/atoms/icons/icon.component';
import { TypographyComponent } from '../../../shared/components/atoms/typography/typography.component';
import { InputComponent } from '../../../shared/components/atoms/inputs/input.component';

@Component({
  selector: 'app-book-edit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonComponent,
    IconComponent,
    TypographyComponent,
    InputComponent
  ],
  templateUrl: './book-edit.component.html',
  styleUrl: './book-edit.component.scss'
})
export class BookEditComponent implements OnInit, OnChanges {
  private bookService = inject(BookService);
  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastService = inject(ToastService);

  // États du composant
  activeTab: 'manual' | 'google' = 'manual';
  isEditMode = false;
  bookId: string | null = null;
  book: Book | null = null;

  // États de chargement
  loading = false;
  categoriesLoading = false;

  // Données
  categories: Category[] = [];

  // Formulaire manuel
  bookForm!: FormGroup;

  // États pour Google Books
  googleBooksResults: GoogleBookPreview[] = [];
  googleBooksLoading = false;
  selectedGoogleBook: GoogleBookPreview | null = null;
  googleSearchQuery = '';
  importCopies = 1;
  selectedCategoryId = '';

  ngOnInit(): void {
    this.initComponent();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['book'] && this.bookForm) {
      this.patchFormValues();
    }
  }

  private initComponent(): void {
    this.loadCategories();
    this.initForm();
    
    // Vérifier si on est en mode édition
    this.route.params.subscribe(params => {
      console.log('Route params received:', params);
      if (params['id']) {
        this.bookId = params['id'];
        this.isEditMode = true;
        console.log('Edit mode activated, bookId:', this.bookId);
        this.loadBook();
      } else {
        console.log('No book ID - creation mode');
      }
    });

    // Vérifier le mode d'onglet depuis les query params
    this.route.queryParams.subscribe(params => {
      if (params['mode'] === 'google') {
        this.activeTab = 'google';
      }
    });
  }

  private loadCategories(): void {
    this.categoriesLoading = true;
    this.bookService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.categoriesLoading = false;
      },
      error: (error) => {
        this.toastService.error('Erreur lors du chargement des catégories');
        this.categoriesLoading = false;
        console.error('Erreur chargement catégories:', error);
      }
    });
  }

  private loadBook(): void {
    if (!this.bookId) {
      console.log('No book ID provided for loading');
      return;
    }

    console.log('Loading book with ID:', this.bookId);
    this.loading = true;
    this.bookService.getBookById(this.bookId).subscribe({
      next: (book) => {
        console.log('Book loaded successfully:', book);
        this.book = book;
        console.log('Book data before patching form:', this.book);
        this.patchFormValues();
        this.loading = false;
        console.log('Form values after patching:', this.bookForm.value);
      },
      error: (error) => {
        console.error('Error loading book:', error);
        this.toastService.error('Erreur lors du chargement du livre');
        this.loading = false;
      }
    });
  }

  private initForm(): void {
    console.log('=== INITIALIZING FORM ===');
    console.log('Edit mode:', this.isEditMode);
    console.log('Current book:', this.book);
    
    this.bookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(1)]),
      author: new FormControl('', [Validators.required, Validators.minLength(1)]),
      isbn: new FormControl('', [
        Validators.required,
        Validators.pattern(/^(?:\d{10}|\d{13})$/)
      ]),
      description: new FormControl('', [Validators.required]),
      publisher: new FormControl('', [Validators.required]),
      publicationDate: new FormControl('', [Validators.required]),
      category: new FormControl('', [Validators.required]),
      totalCopies: new FormControl(1, [
        Validators.required,
        Validators.min(1),
        Validators.max(1000)
      ]),
      coverUrl: new FormControl(''),
      pageCount: new FormControl(null, [Validators.min(1)])
    });

    console.log('Form initialized with controls:', Object.keys(this.bookForm.controls));
    console.log('Initial form values:', this.bookForm.value);
    
    // Si on est en mode édition et que le livre est déjà chargé, patcher immédiatement
    if (this.isEditMode && this.book) {
      console.log('Book already loaded, patching form immediately');
      this.patchFormValues();
    }
  }

  private patchFormValues(): void {
    if (this.book && this.bookForm) {
      console.log('=== PATCHING FORM VALUES ===');
      console.log('Book data received:', this.book);
      console.log('Book keys:', Object.keys(this.book));
      
      // Vérifier chaque propriété du livre
      console.log('Title:', this.book.title);
      console.log('Author:', this.book.author);
      console.log('ISBN:', this.book.isbn);
      console.log('Description:', this.book.description);
      console.log('Publisher:', this.book.publisher);
      console.log('PublicationDate:', this.book.publicationDate);
      console.log('Category:', this.book.category);
      console.log('CoverUrl:', this.book.coverUrl);
      console.log('PageCount:', this.book.pageCount);
      console.log('TotalCopies:', this.book.totalCopies);
      
      const patchData: any = {
        title: this.book.title || '',
        author: this.book.author || '',
        isbn: this.book.isbn || '',
        description: this.book.description || '',
        publisher: this.book.publisher || '',
        category: this.book.category || '',
        coverUrl: this.book.coverUrl || '',
        pageCount: this.book.pageCount || null,
        totalCopies: this.book.totalCopies || 1
      };

      // Gestion de la date de publication
      if (this.book.publicationDate) {
        try {
          const date = new Date(this.book.publicationDate);
          if (!isNaN(date.getTime())) {
            patchData.publicationDate = date.toISOString().split('T')[0];
          } else {
            patchData.publicationDate = '';
          }
        } catch (error) {
          console.error('Error parsing publication date:', error);
          patchData.publicationDate = '';
        }
      } else {
        patchData.publicationDate = '';
      }

      console.log('=== PATCH DATA ===');
      console.log('Data to patch:', patchData);
      
      // Vérifier chaque contrôle du formulaire avant le patch
      Object.keys(patchData).forEach(key => {
        const control = this.bookForm.get(key);
        console.log(`Control '${key}':`, control ? 'EXISTS' : 'MISSING', 'Value:', patchData[key]);
      });

      this.bookForm.patchValue(patchData);
      
      console.log('=== FORM AFTER PATCHING ===');
      console.log('Form values:', this.bookForm.value);
      
      // Vérifier chaque contrôle après le patch
      Object.keys(this.bookForm.controls).forEach(key => {
        const control = this.bookForm.get(key);
        console.log(`Control '${key}' value:`, control?.value);
      });
    } else {
      console.log('Cannot patch form - book or form is missing');
      console.log('Book:', this.book);
      console.log('Form:', this.bookForm);
    }
  }

  // Gestion des onglets
  setActiveTab(tab: 'manual' | 'google'): void {
    this.activeTab = tab;
  }

  // Soumission du formulaire manuel
  onSubmitManual(): void {
    if (this.bookForm.invalid || this.loading) {
      this.markFormGroupTouched();
      return;
    }

    const formData = this.bookForm.value;
    this.loading = true;

    // Debug: Afficher les données du formulaire
    console.log('Données du formulaire:', formData);
    console.log('Valeur de coverUrl:', formData.coverUrl);

    // Convertir la date au format ISO 8601 complet
    const publicationDate = formData.publicationDate ?
      new Date(formData.publicationDate + 'T00:00:00.000Z').toISOString() :
      undefined;

    if (this.isEditMode && this.bookId) {
      const updateData: UpdateBookRequest = {
        title: formData.title,
        author: formData.author,
        isbn: formData.isbn,
        description: formData.description,
        publisher: formData.publisher,
        publicationDate: publicationDate,
        category: formData.category,
        coverUrl: formData.coverUrl || '',
        pageCount: formData.pageCount
      };
      
      console.log('Données de mise à jour envoyées:', updateData);
      
      this.bookService.updateBook(this.bookId, updateData).subscribe({
        next: (book) => {
          this.toastService.success('Livre mis à jour avec succès');
          this.loading = false;
          this.goBack();
        },
        error: (error) => {
          this.toastService.error('Erreur lors de la mise à jour du livre');
          this.loading = false;
          console.error('Erreur mise à jour livre:', error);
        }
      });
    } else {
      const createData: CreateBookRequest = {
        title: formData.title,
        author: formData.author,
        isbn: formData.isbn,
        description: formData.description,
        publisher: formData.publisher,
        publicationDate: publicationDate,
        category: formData.category,
        totalCopies: formData.totalCopies,
        coverUrl: formData.coverUrl || '',
        pageCount: formData.pageCount
      };
      
      console.log('Données de création envoyées:', createData);
      
      this.bookService.createBook(createData).subscribe({
        next: (book) => {
          this.toastService.success('Livre créé avec succès');
          this.loading = false;
          this.goBack();
        },
        error: (error) => {
          this.toastService.error('Erreur lors de la création du livre');
          this.loading = false;
          console.error('Erreur création livre:', error);
        }
      });
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.bookForm.controls).forEach(key => {
      const control = this.bookForm.get(key);
      control?.markAsTouched();
    });
  }

  // Recherche Google Books
  searchGoogleBooks(): void {
    if (!this.googleSearchQuery.trim()) {
      this.toastService.error('Veuillez entrer un terme de recherche');
      return;
    }

    this.googleBooksLoading = true;
    this.googleBooksResults = [];
    this.selectedGoogleBook = null;

    this.bookService.searchGoogleBooks(this.googleSearchQuery).subscribe({
      next: (results) => {
        this.googleBooksResults = results;
        this.googleBooksLoading = false;
      },
      error: (error) => {
        this.toastService.error('Erreur lors de la recherche Google Books');
        this.googleBooksLoading = false;
        console.error('Erreur recherche Google Books:', error);
      }
    });
  }

  // Sélection d'un livre Google
  selectGoogleBook(book: GoogleBookPreview): void {
    this.selectedGoogleBook = book;
  }

  // Remplir le formulaire manuel avec les données Google
  fillFormWithGoogleData(): void {
    if (!this.selectedGoogleBook) return;

    this.bookForm.patchValue({
      title: this.selectedGoogleBook.title,
      author: this.selectedGoogleBook.authors?.join(', ') || '',
      isbn: this.selectedGoogleBook.isbn,
      description: this.selectedGoogleBook.description,
      publisher: this.selectedGoogleBook.publisher,
      publicationDate: this.selectedGoogleBook.publishedDate,
      coverUrl: this.selectedGoogleBook.coverUrl,
      pageCount: this.selectedGoogleBook.pageCount
    });

    // Basculer vers l'onglet manuel
    this.activeTab = 'manual';
  }

  // Importation directe depuis Google Books
  importGoogleBook(): void {
    if (!this.selectedGoogleBook || !this.selectedCategoryId) {
      this.toastService.error('Veuillez sélectionner un livre et une catégorie');
      return;
    }

    const importRequest: ImportBookFromGoogleRequest = {
      isbn: this.selectedGoogleBook.isbn,
      totalCopies: this.importCopies,
      defaultCategory: this.selectedCategoryId
    };

    this.loading = true;
    this.bookService.importFromGoogle(importRequest).subscribe({
      next: (book) => {
        this.toastService.success('Livre importé avec succès depuis Google Books');
        this.loading = false;
        this.goBack();
      },
      error: (error) => {
        this.toastService.error('Erreur lors de l\'importation du livre');
        this.loading = false;
        console.error('Erreur importation livre:', error);
      }
    });
  }

  // Retour à la page d'administration
  goBack(): void {
    this.router.navigate(['/admin/books']);
  }

  // Annulation
  onCancel(): void {
    this.goBack();
  }

  // Getters pour les erreurs du formulaire
  getFieldError(fieldName: string): string | null {
    const control = this.bookForm.get(fieldName);
    if (control?.invalid && control.touched) {
      if (control.hasError('required')) {
        return 'Ce champ est requis';
      }
      if (control.hasError('minlength')) {
        return 'Ce champ est trop court';
      }
      if (control.hasError('pattern')) {
        return 'Format ISBN invalide (10 ou 13 chiffres)';
      }
      if (control.hasError('min')) {
        return 'La valeur doit être supérieure à 0';
      }
      if (control.hasError('max')) {
        return 'La valeur est trop élevée';
      }
    }
    return null;
  }

  // Vérification de validité du formulaire
  isFormValid(): boolean {
    return this.bookForm.valid;
  }
}
