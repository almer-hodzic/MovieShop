export interface AddFavouriteMovieCommand {
  movieId: number;
}

export interface GetMyFavouriteMoviesQueryDto {
  favouriteMovieId: number;
  movieId: number;
  dateAdded: string;
  title: string;
  releaseDate: string;
  creationDate: string;
  duration: number;
  directorId: number;
  directorName: string;
  countryId: number;
  trailerLink?: string | null;
  image?: string | null;
  storyLine: string;
  price: number;
  averageScore?: number | null;
}
