import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css']
})
export class BookComponent implements OnInit {

    public comics: any[];

    constructor(public http: Http) {
    }

    ngOnInit() {
        this.http.get("/api/book/comics").subscribe(result => {
            this.comics = result.json().collection.map(comic => ({
                name: comic.name,
                thumbnail: 'data:image/png;base64,' + comic.thumbnail
            }));
        });
    }

}
