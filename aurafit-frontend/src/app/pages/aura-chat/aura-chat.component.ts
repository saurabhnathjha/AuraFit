import { Component } from '@angular/core';
import { NgFor, NgIf, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AuraChatService, CreateQueryDTO } from '../../services/aura-chat.service';

@Component({
  selector: 'app-aura-chat',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    NgClass,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
  ],
  templateUrl: './aura-chat.component.html',
  styleUrl: './aura-chat.component.css'
})
export class AuraChatComponent {
  userInput: string = '';
  messages: { text: string; sender: 'user' | 'bot' }[] = [];
  isLoading: boolean = false;

  constructor(private chatbotService: AuraChatService) { }

  sendMessage() {
    const input = this.userInput.trim();
    if (!input) return;

    this.messages.push({ text: input, sender: 'user' });
    this.userInput = '';
    this.isLoading = true;

    const dto: CreateQueryDTO = { query: input };

    this.chatbotService.askQuestion(dto).subscribe({
      next: (res) => {
        const reply = res?.response || 'Sorry, I didn\'t get that.';
        this.messages.push({ text: reply, sender: 'bot' });
        this.isLoading = false;
      },
      error: () => {
        this.messages.push({ text: 'An error occurred. Please try again.', sender: 'bot' });
        this.isLoading = false;
      }
    });
  }
}
