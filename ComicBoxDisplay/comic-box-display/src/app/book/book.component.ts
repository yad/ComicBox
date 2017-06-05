import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'app-book',
    templateUrl: './book.component.html',
    styleUrls: ['./book.component.css']
})
export class BookComponent implements OnInit {

    public books: any[];

    constructor(private http: Http) {
    }

    ngOnInit() {
        this.books = [];
        this.http.get("/api/book/comics").subscribe(response => {
            const result = response.json();
            const collection = result.collection.map(book => ({
                name: book.name,
                thumbnail: book.thumbnail ? `data:image/png;base64,${book.thumbnail}` : '/assets/nopreview.jpg'
            }));
            this.books.push(...collection);
            this.callApi(1);
        });
    }

    private callApi(pagination: number) {
        this.http.get(`/api/thumbnail/comics/${pagination}`).subscribe(response => {
            const result = response.json();
            const collection = result.collection.map(book => ({
                name: book.name,
                thumbnail: book.thumbnail ? `data:image/png;base64,${book.thumbnail}` : '/assets/nopreview.jpg'
            }));

            collection.forEach(book => {
                let currentBook = this.books.filter(b => b.name === book.name)[0];
                if (currentBook) {
                    currentBook.thumbnail = book.thumbnail;
                }
            });
                        
            if (result.hasNextPagination) {
                this.callApi(pagination + 1);
            }
        });
    }

}
