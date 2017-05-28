import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.css']
})
export class PageComponent implements OnInit {

  public image: string;

  constructor(public http: Http) {
  }

  ngOnInit() {
      this.http.get("/api/book/comics/Ant-Man/007.pdf/001").subscribe(result => {
          this.image = 'data:image/png;base64,' + result.json().page;
      });
  }

}
