﻿namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Net.NetworkInformation;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here

            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Include(a => a.Producer)
                .Select(a => new
                {
                    Name = a.Name,
                    ReleaseDate = a.ReleaseDate
                    .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    TotalPrice = a.Price,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price,
                        WriterName = s.Writer.Name,
                    })
                })
            .ToList();

            StringBuilder sb = new();
            foreach (var album in albums.OrderByDescending(a => a.TotalPrice))
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                int counter = 1;
                foreach (var song in album.Songs.OrderByDescending(o => o.SongName).ThenBy(o => o.WriterName).ToList())
                {
                    sb.AppendLine($"---#{counter}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                    counter++;
                }
                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Where(song => song.Duration > TimeSpan.FromSeconds(duration))
                .Select(s => new
                {
                    Name = s.Name,
                    Performers = s.SongPerformers.ToList()
                    .Where(sb => sb.SongId == s.Id),
                    Writer = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .ToList();

            int counter = 1;
            StringBuilder sb = new();
            foreach (var song in songs.OrderBy(s => s.Name).ThenBy(s => s.Writer))
            {
                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                if (song.Performers.Any())
                {
                    foreach (var performer in song.Performers.OrderBy(p => p.Performer.FirstName + " " + p.Performer.LastName))
                    {
                        sb.AppendLine($"---Performer: {performer.Performer.FirstName} {performer.Performer.LastName}");
                    }
                }
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
                counter++;
            }

            return sb.ToString().TrimEnd();   
        }
    }
}
