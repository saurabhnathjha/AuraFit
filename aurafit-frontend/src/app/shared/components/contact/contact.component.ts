import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import emailjs from 'emailjs-com';

@Component({
  selector: 'app-contact',
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatTooltipModule

  ],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent {
  currentDate = new Date().toLocaleDateString();
  onSubmit(e: Event) {
    e.preventDefault(); // prevent page reload
    emailjs.sendForm(
      'service_ace',     // ✅ Your service ID
      'template_gd6n07n',    // ✅ Your template ID
      e.target as HTMLFormElement,
      'FiJyJ52qjjLxkI7LB'    // ✅ Your public key
    ).then(
      result => {
        console.log('SUCCESS!', result.text);
        alert('Message sent successfully!');
      },
      error => {
        console.error('FAILED...', error.text);
        alert('Oops! Something went wrong.');
      }
    );
  }

}
