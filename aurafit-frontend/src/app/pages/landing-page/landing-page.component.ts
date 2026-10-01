import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, FormsModule, RouterModule],
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css'],
})
export class LandingPageComponent implements OnInit{
  // Feature highlight cards
  heroImages: string[] = [
    '/hero1.png',
    '/hero2.png',
    '/hero3.png'
  ];
  currentImageIndex = 0;
  ngOnInit() {
    setInterval(() => {
      this.currentImageIndex = (this.currentImageIndex + 1) % this.heroImages.length;
    }, 7000); 
  }

  get currentImage(): string {
    return this.heroImages[this.currentImageIndex];
  }
  constructor(private router: Router) { }

  features = [
    {
      title: 'Log Workouts',
      description: 'Track every rep, set, and session with ease and precision.',
      image: '/services-1.jpg'
    },
    {
      title: 'Track Meals',
      description: 'Stay on top of your diet with detailed meal logging.',
      image: '/services-3.jpg'
    },
    {
      title: 'Aura Chatbot',
      description: 'Your AI fitness assistant, available 24/7 for guidance.',
      image: '/services-2.jpg'
    }
  ];

  // BMI calculator data
  height!: number;
  weight!: number;
  bmiResult!: number | null;
  bmiMessage: string = '';
  bmiClass: string = '';

  calculateBMI(): void {
    if (this.height && this.weight) {
      const heightInMeters = this.height / 100;
      const bmi = this.weight / (heightInMeters * heightInMeters);
      this.bmiResult = parseFloat(bmi.toFixed(1));

      if (bmi < 18.5) {
        this.bmiMessage = "You are underweight. Join AuraFit today and explore exciting ways to add on some healthy weight!";
        this.bmiClass = 'underweight';
      } else if (bmi < 24.9) {
        this.bmiMessage = "Great job! You have a healthy weight. Join AuraFit today to stay consistent and level up your fitness journey!";
        this.bmiClass = 'normal';
      } else if (bmi < 29.9) {
        this.bmiMessage = "You're slightly overweight. AuraFit can help you get back on track with expert guidance and smart workouts!";
        this.bmiClass = 'overweight';
      } else {
        this.bmiMessage = "Your BMI indicates obesity. AuraFit is here to help you transform your health, one step at a time.";
        this.bmiClass = 'obese';
      }
    }
  }
}
