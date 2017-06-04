import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'app-chapter',
    templateUrl: './chapter.component.html',
    styleUrls: ['./chapter.component.css']
})
export class ChapterComponent implements OnInit {

    public chapters: any[];

    public book: string;

    constructor(private route: ActivatedRoute, private http: Http) {
    }

    ngOnInit() {
        this.chapters = [];

        this.route.params.subscribe(params => {
            this.book = params["book"];
            this.callApi(1);
        });
    }

    private callApi(pagination: number) {
        return this.http.get(`/api/book/comics/${this.book}/${pagination}`).subscribe(response => {
            const result = response.json();
            const collection = result.collection.map(chapter => ({
                name: chapter.name,
                displayName: `Tome #${parseInt(chapter.name.slice(0, -4))}`,
                thumbnail: chapter.thumbnail ? `data:image/png;base64,${chapter.thumbnail}` : '/assets/nopreview.jpg'
            }));

            this.chapters.push(...collection);
            if (result.hasNextPagination) {
                this.callApi(pagination + 1);
            }
        });
    }
}
