﻿using GovUk.Education.ExploreEducationStatistics.Common.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
using static GovUk.Education.ExploreEducationStatistics.Data.Model.Migrations.MigrationConstants;

namespace GovUk.Education.ExploreEducationStatistics.Data.Model.Migrations
{
    public partial class EES1830_MigrateFinalObservationsAndObservationFilterItems : Migration
    {
        private const string MigrationId = "20210302050000";
        private const string PreviousMigrationId = "20210126094900";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlFromFile(MigrationsPath, $"{MigrationId}_MigrateFinalObservationsAndObservationFilterItems.sql");
            migrationBuilder.Sql("DROP PROCEDURE MigrateObservationsAndObservationFilterItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlFromFile(MigrationsPath, $"{PreviousMigrationId}_Routine_MigrateObservationsAndObservationFilterItems.sql");
        }
    }
}