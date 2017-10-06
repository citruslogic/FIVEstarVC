import { Component } from '@angular/core';
@Component({
    selector: 'my-app',
    template: `    
    <h2>FIVE STAR Veterans Center</h2>
    <p>This is the Resident Management System SNAPSHOT</p>
   
  `
})
export class AppComponent {
    title = 'ASP.NET MVC 5 with Angular 2';
    skills = ['MVC 5', 'Angular 2', 'TypeScript', 'Visual Studio 2015'];
    myskills = this.skills[1];
}