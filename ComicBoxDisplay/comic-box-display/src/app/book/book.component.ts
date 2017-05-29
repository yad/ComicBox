import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'app-book',
    templateUrl: './book.component.html',
    styleUrls: ['./book.component.css']
})
export class BookComponent implements OnInit {

    public books: any[];

    constructor(public http: Http) {
    }

    ngOnInit() {
        this.http.get("/api/book/comics").subscribe(result => {
            this.books = result.json().collection.map(book => ({
                name: book.name,
                thumbnail: 'data:image/png;base64,' + book.thumbnail
            }));
        });
    }

}
