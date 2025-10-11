import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../../atoms/icons/icon.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { TypographyComponent } from '../../atoms/typography/typography.component'; 
import { InputComponent } from '../../atoms/inputs/input.component';
import { Book, Category, CreateBookRequest, UpdateBookRequest } from '../../../../core/models/book.model';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, IconComponent, ButtonComponent, TypographyComponent, InputComponent],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.scss'
})
export class BookFormComponent {
 @Input() book: Book | null = null;
  @Input() categories: Category[] = [];
  @Input() loading = false;
  @Input() errorMessage: string | null = null;

  @Output() formSubmit = new EventEmitter<CreateBookRequest | UpdateBookRequest>();
  @Output() importFromGoogle = new EventEmitter<string>();

  bookForm!: FormGroup;
  isEditMode = false;

  ngOnInit(): void {
    this.isEditMode = !!this.book;
    this.initForm();
  }

  private initForm(): void {
    const defaultValues = this.isEditMode
      ? {
          title: this.book!.title,
          author: this.book!.author,
          isbn: this.book!.isbn,
          description: this.book!.description,
          publisher: this.book!.publisher,
          publicationDate: this.book!.publicationDate
            ? new Date(this.book!.publicationDate).toISOString().split('T')[0]
            : '',
          categoryId: this.book!.category,
          totalCopies: this.book!.totalCopies,
          coverUrl: this.book!.coverUrl
        }
      : {
          title: '',
          author: '',
          isbn: '',
          description: '',
          publisher: '',
          publicationDate: '',
          categoryId: '',
          totalCopies: 1,
          coverUrl: ''
        };

    this.bookForm = new FormGroup({
      title: new FormControl(defaultValues.title, [Validators.required]),
      author: new FormControl(defaultValues.author, [Validators.required]),
      isbn: new FormControl(defaultValues.isbn, [
        Validators.required,
        Validators.pattern(/^\d{10}(\d{3})?$/)
      ]),
      description: new FormControl(defaultValues.description, [Validators.required]),
      publisher: new FormControl(defaultValues.publisher, [Validators.required]),
      publicationDate: new FormControl(defaultValues.publicationDate, [Validators.required]),
      categoryId: new FormControl(defaultValues.categoryId, [Validators.required]),
      totalCopies: new FormControl(defaultValues.totalCopies, [
        Validators.required,
        Validators.min(1)
      ]),
      coverUrl: new FormControl(defaultValues.coverUrl)
    });
  }

  onSubmit(): void {
    if (this.bookForm.invalid || this.loading) return;

    const formData = this.bookForm.value;

    if (this.isEditMode) {
      const updateData: UpdateBookRequest = {
        title: formData.title,
        author: formData.author,
        isbn: formData.isbn,
        description: formData.description,
        publisher: formData.publisher,
        publicationDate: formData.publicationDate,
        category: formData.categoryId,
        coverUrl: formData.coverUrl
      };
      this.formSubmit.emit(updateData);
    } else {
      const createData: CreateBookRequest = {
        title: formData.title,
        author: formData.author,
        isbn: formData.isbn,
        description: formData.description,
        publisher: formData.publisher,
        publicationDate: formData.publicationDate,
        category: formData.categoryId,
        totalCopies: formData.totalCopies,
        coverUrl: formData.coverUrl
      };
      this.formSubmit.emit(createData);
    }
  }

  onImportFromGoogle(): void {
    const isbn = this.bookForm.get('isbn')?.value?.trim();
    if (isbn) {
      this.importFromGoogle.emit(isbn);
    }
  }
}
