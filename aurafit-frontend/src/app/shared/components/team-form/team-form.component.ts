import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-form.component.html',
  styleUrls: ['./team-form.component.css']
})
export class TeamFormComponent implements OnInit {
  teamForm!: FormGroup;
  teamMembers: any[] = [];

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    // Initialize form
    this.teamForm = this.fb.group({
      name: [''],
      role: [''],
      contribution: [''],
      linkedin: ['']
    });

    // Load existing data from localStorage
    const data = localStorage.getItem('teamData');
    if (data) {
      this.teamMembers = JSON.parse(data);
    }
  }

  onSubmit() {
    if (this.teamForm.valid) {
      const newMember = this.teamForm.value;
      this.teamMembers.push(newMember);
      localStorage.setItem('teamData', JSON.stringify(this.teamMembers));
      this.teamForm.reset();
      alert('Team member added successfully!');
    }
  }
}
