import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

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
        this.books = [];
        this.callApi(1);
    }

    private callApi(pagination: number) {
        this.http.get(`/api/book/comics/${pagination}`).subscribe(response => {
            const result = response.json();
            const collection = result.collection.map(book => ({
                name: book.name,
                thumbnail: `data:image/png;base64,${book.thumbnail}`
            }));

            this.books.push(...collection);
            if (result.hasNextPagination) {
                this.callApi(pagination + 1);
            }
        });
    }

}
