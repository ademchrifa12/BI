import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, CreateUser, UpdateUser } from '../../../services/user.service';
import { User } from '../../../models/models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  loading = true;
  error = '';
  showModal = false;
  modalMode: 'create' | 'edit' = 'create';
  saving = false;
  deleteConfirmId: number | null = null;

  formData = {
    username: '',
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    role: 'User',
    isActive: true
  };
  editUserId: number | null = null;
  formError = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.error = '';
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load users';
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.modalMode = 'create';
    this.formData = { username: '', email: '', password: '', firstName: '', lastName: '', role: 'User', isActive: true };
    this.editUserId = null;
    this.formError = '';
    this.showModal = true;
  }

  openEditModal(user: User): void {
    this.modalMode = 'edit';
    this.formData = {
      username: user.username,
      email: user.email,
      password: '',
      firstName: user.firstName,
      lastName: user.lastName,
      role: user.role,
      isActive: user.isActive
    };
    this.editUserId = user.userId;
    this.formError = '';
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.formError = '';
  }

  saveUser(): void {
    this.formError = '';

    if (!this.formData.username || !this.formData.email || !this.formData.firstName || !this.formData.lastName) {
      this.formError = 'All fields are required';
      return;
    }

    if (this.modalMode === 'create' && !this.formData.password) {
      this.formError = 'Password is required for new users';
      return;
    }

    this.saving = true;

    if (this.modalMode === 'create') {
      const dto: CreateUser = {
        username: this.formData.username,
        email: this.formData.email,
        password: this.formData.password,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        role: this.formData.role
      };
      this.userService.create(dto).subscribe({
        next: () => {
          this.saving = false;
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.saving = false;
          this.formError = err.error?.message || 'Failed to create user';
        }
      });
    } else {
      const dto: UpdateUser = {
        username: this.formData.username,
        email: this.formData.email,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        role: this.formData.role,
        isActive: this.formData.isActive,
        password: this.formData.password || undefined
      };
      this.userService.update(this.editUserId!, dto).subscribe({
        next: () => {
          this.saving = false;
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.saving = false;
          this.formError = err.error?.message || 'Failed to update user';
        }
      });
    }
  }

  confirmDelete(userId: number): void {
    this.deleteConfirmId = userId;
  }

  cancelDelete(): void {
    this.deleteConfirmId = null;
  }

  deleteUser(userId: number): void {
    this.userService.delete(userId).subscribe({
      next: () => {
        this.deleteConfirmId = null;
        this.loadUsers();
      },
      error: () => {
        this.error = 'Failed to delete user';
        this.deleteConfirmId = null;
      }
    });
  }
}
