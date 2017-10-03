using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Model;

namespace events.dal.Models
{
    [Migration(20170926)]
    public class Init : AutoReversingMigration
    {
        public override void Up()
        {

            this.Create.Table("Event")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().WithDefaultValue(SystemMethods.NewGuid)
                .WithColumn("SourceUri").AsString(500).Nullable()
                
                .WithColumn("Uri").AsString(500).Nullable()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Summary").AsString(500).NotNullable()
                .WithColumn("Description").AsString(4000).Nullable()
                .WithColumn("EventType").AsString(500).Nullable()
                .WithColumn("StartDate").AsString(20).NotNullable()
                .WithColumn("EndDate").AsString(20).Nullable()
                .WithColumn("ImageUri").AsString(500).Nullable()
                .WithColumn("Performer").AsString(500).Nullable()
                .WithColumn("Offers").AsString(500).Nullable()
                // location
                .WithColumn("LocationUri").AsString(500).Nullable()
                .WithColumn("LocationName").AsString(500).Nullable()
                .WithColumn("Street").AsString(200).Nullable()
                .WithColumn("Zip").AsString(20).Nullable()
                .WithColumn("Locality").AsString(200).Nullable()
                .WithColumn("Region").AsString(200).Nullable()
                .WithColumn("Country").AsString(200).Nullable()
                .WithColumn("Longitude").AsDecimal(12, 9).Nullable()
                .WithColumn("Latitude").AsDecimal(12, 9).Nullable()
                ;

        }
    }
}
