import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  teamMembers: any[] = [];

  ngOnInit() {
    const data = localStorage.getItem('teamData');
    if (data) {
      this.teamMembers = JSON.parse(data);
    }
  }

  deleteMember(index: number) {
    if (confirm('Are you sure you want to delete this team member?')) {
      this.teamMembers.splice(index, 1);
      localStorage.setItem('teamData', JSON.stringify(this.teamMembers));
    }
  }

  editMember(index: number) {
    const member = this.teamMembers[index];
    const name = prompt('Edit Name:', member.name);
    const role = prompt('Edit Role:', member.role);
    const contribution = prompt('Edit Contribution:', member.contribution);
    const linkedin = prompt('Edit LinkedIn:', member.linkedin);

    if (name && role && contribution && linkedin) {
      this.teamMembers[index] = { name, role, contribution, linkedin };
      localStorage.setItem('teamData', JSON.stringify(this.teamMembers));
    }
  }
}
