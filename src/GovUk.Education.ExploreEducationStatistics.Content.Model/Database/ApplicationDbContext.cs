using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Data.Model.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

// ReSharper disable StringLiteralTypo
namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        private enum FilterItemName
        {
            Characteristic__Total,
            School_Type__Total,
            Year_of_admission__Primary_All_primary,
            Year_of_admission__Secondary_All_secondary
        }

        private enum IndicatorName
        {
            Unauthorised_absence_rate,
            Overall_absence_rate,
            Authorised_absence_rate,
            Number_of_schools,
            Number_of_pupils,
            Number_of_permanent_exclusions,
            Permanent_exclusion_rate,
            Number_of_fixed_period_exclusions,
            Fixed_period_exclusion_rate,
            Percentage_of_pupils_with_fixed_period_exclusions,
            Number_of_admissions,
            Number_of_applications_received,
            Number_of_first_preferences_offered,
            Number_of_second_preferences_offered,
            Number_of_third_preferences_offered,
            Number_that_received_one_of_their_first_three_preferences,
            Number_that_received_an_offer_for_a_preferred_school,
            Number_that_received_an_offer_for_a_non_preferred_school,
            Number_that_did_not_receive_an_offer,
            Number_that_received_an_offer_for_a_school_within_their_LA
        }

        private static readonly Dictionary<int, Dictionary<FilterItemName, int>> SubjectFilterItemIds =
            new Dictionary<int, Dictionary<FilterItemName, int>>
            {
                {
                    1, new Dictionary<FilterItemName, int>
                    {
                        {
                            FilterItemName.Characteristic__Total, 1
                        },
                        {
                            FilterItemName.School_Type__Total, 58
                        }
                    }
                },
                {
                    12, new Dictionary<FilterItemName, int>
                    {
                        {
                            FilterItemName.School_Type__Total, 461
                        }
                    }
                },
                {
                    17, new Dictionary<FilterItemName, int>
                    {
                        {
                            FilterItemName.Year_of_admission__Primary_All_primary, 575
                        },
                        {
                            FilterItemName.Year_of_admission__Secondary_All_secondary, 577
                        }
                    }
                }
            };

        private static readonly Dictionary<int, Dictionary<IndicatorName, int>> SubjectIndicatorIds =
            new Dictionary<int, Dictionary<IndicatorName, int>>
            {
                {
                    1, new Dictionary<IndicatorName, int>
                    {
                        {
                            IndicatorName.Unauthorised_absence_rate, 23
                        },
                        {
                            IndicatorName.Overall_absence_rate, 26
                        },
                        {
                            IndicatorName.Authorised_absence_rate, 28
                        }
                    }
                },
                {
                    12, new Dictionary<IndicatorName, int>
                    {
                        {
                            IndicatorName.Number_of_schools, 176
                        },
                        {
                            IndicatorName.Number_of_pupils, 177
                        },
                        {
                            IndicatorName.Number_of_permanent_exclusions, 178
                        },
                        {
                            IndicatorName.Permanent_exclusion_rate, 179
                        },
                        {
                            IndicatorName.Number_of_fixed_period_exclusions, 180
                        },
                        {
                            IndicatorName.Fixed_period_exclusion_rate, 181
                        },
                        {
                            IndicatorName.Percentage_of_pupils_with_fixed_period_exclusions, 183
                        }
                    }
                },
                {
                    17, new Dictionary<IndicatorName, int>
                    {
                        {
                            IndicatorName.Number_of_admissions, 211
                        },
                        {
                            IndicatorName.Number_of_applications_received, 212
                        },
                        {
                            IndicatorName.Number_of_first_preferences_offered, 216
                        },
                        {
                            IndicatorName.Number_of_second_preferences_offered, 217
                        },
                        {
                            IndicatorName.Number_of_third_preferences_offered, 218
                        },
                        {
                            IndicatorName.Number_that_received_one_of_their_first_three_preferences, 219
                        },
                        {
                            IndicatorName.Number_that_received_an_offer_for_a_preferred_school, 220
                        },
                        {
                            IndicatorName.Number_that_received_an_offer_for_a_non_preferred_school, 221
                        },
                        {
                            IndicatorName.Number_that_did_not_receive_an_offer, 222
                        },
                        {
                            IndicatorName.Number_that_received_an_offer_for_a_school_within_their_LA, 223
                        }
                    }
                }
            };

        public DbSet<Methodology> Methodologies { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Release> Releases { get; set; }
        
        public DbSet<ReleaseSummary> ReleaseSummaries { get; set; }
        
        public DbSet<ReleaseSummaryVersion> ReleaseSummaryVersions { get; set; }
        public DbSet<ReleaseType> ReleaseTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Methodology>()
                .Property(b => b.Content)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<ContentSection>>(v));
            modelBuilder.Entity<Methodology>()
                .Property(b => b.Annexes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<ContentSection>>(v));

            modelBuilder.Entity<Publication>()
                .Property(p => p.LegacyPublicationUrl)
                .HasConversion(
                    p => p.ToString(),
                    p => new Uri(p));
            
            modelBuilder.Entity<Release>()
                .Property(r => r.TimePeriodCoverage)
                .HasConversion(new EnumToEnumValueConverter<TimeIdentifier>())
                .HasMaxLength(6);
            
            modelBuilder.Entity<Release>()
                .Property(b => b.Content)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<ContentSection>>(v));
            
            modelBuilder.Entity<Release>()
                .Property(b => b.NextReleaseDate)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PartialDate>(v));

            modelBuilder.Entity<Release>()
                .Property(b => b.KeyStatistics)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<DataBlock>(v));
            
            modelBuilder.Entity<Release>()
                .Property(b => b.Status)
                .HasConversion(new EnumToStringConverter<ReleaseStatus>());

            modelBuilder.Entity<ReleaseSummary>()
                .HasOne(rs => rs.Release).WithOne(r => r.ReleaseSummary)
                .HasForeignKey<ReleaseSummary>(rs => rs.ReleaseId);

            modelBuilder.Entity<ReleaseSummaryVersion>()
                .HasOne(rsv => rsv.ReleaseSummary)
                .WithMany(rs => rs.Versions)
                .HasForeignKey(rsv => rsv.ReleaseSummaryId);
            
            modelBuilder.Entity<ReleaseSummaryVersion>()
                .Property(b => b.NextReleaseDate)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PartialDate>(v));
            
            modelBuilder.Entity<ReleaseSummaryVersion>()
                .Property(r => r.TimePeriodCoverage)
                .HasConversion(new EnumToEnumValueConverter<TimeIdentifier>())
                .HasMaxLength(6);
            
            modelBuilder.Entity<ReleaseType>().HasData(
                new ReleaseType
                {
                    Id = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    Title = "Official Statistics"
                },
                new ReleaseType
                {
                    Id = new Guid("1821abb8-68b0-431b-9770-0bea65d02ff0"),
                    Title = "Ad Hoc"
                },
                new ReleaseType
                {
                    Id = new Guid("8becd272-1100-4e33-8a7d-1c0c4e3b42b8"),
                    Title = "National Statistics"
                });

            modelBuilder.Entity<Theme>().HasData(
                new Theme
                {
                    Id = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Title = "Children, early years and social care",
                    Summary =
                        "Including children in need, EYFS, and looked after children and social workforce statistics",
                    Slug = "children-and-early-years"
                },
                new Theme
                {
                    Id = new Guid("6412a76c-cf15-424f-8ebc-3a530132b1b3"),
                    Title = "Destination of pupils and students",
                    Summary =
                        "Including graduate labour market and not in education, employment or training (NEET) statistics",
                    Slug = "destination-of-pupils-and-students"
                },
                new Theme
                {
                    Id = new Guid("bc08839f-2970-4f34-af2d-29608a48082f"),
                    Title = "Finance and funding",
                    Summary = "Including local authority (LA) and student loan statistics",
                    Slug = "finance-and-funding"
                },
                new Theme
                {
                    Id = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Title = "Further education",
                    Summary =
                        "Including advanced learner loan, benefit claimant and apprenticeship and traineeship statistics",
                    Slug = "further-education"
                },
                new Theme
                {
                    Id = new Guid("2ca22e34-b87a-4281-a0eb-b80f4f8dd374"),
                    Title = "Higher education",
                    Summary = "Including university graduate employment and participation statistics",
                    Slug = "higher-education"
                },
                new Theme
                {
                    Id = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Title = "Pupils and schools",
                    Summary =
                        "Including absence, application and offers, capacity exclusion and special educational needs (SEN) statistics",
                    Slug = "pupils-and-schools"
                },
                new Theme
                {
                    Id = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Title = "School and college outcomes and performance",
                    Summary = "Including GCSE and key stage statistcs",
                    Slug = "school-and-college-performance"
                },
                new Theme
                {
                    Id = new Guid("b601b9ea-b1c7-4970-b354-d1f695c446f1"),
                    Title = "Teachers and school workforce",
                    Summary = "Including initial teacher training (ITT) statistics",
                    Slug = "teachers-and-school-workforce"
                },
                new Theme
                {
                    Id = new Guid("a95d2ca2-a969-4320-b1e9-e4781112574a"),
                    Title = "UK education and training statistics",
                    Summary =
                        "Including summarised expenditure, post-compulsory education, qualitification and school statistics",
                    Slug = "uk-education-and-training-statistics"
                }
            );

            modelBuilder.Entity<Topic>().HasData(
                new Topic
                {
                    Id = new Guid("1003fa5c-b60a-4036-a178-e3a69a81b852"),
                    Title = "Childcare and early years",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "childcare-and-early-years"
                },
                new Topic
                {
                    Id = new Guid("22c52d89-88c0-44b5-96c4-042f1bde6ddd"),
                    Title = "Children in need and child protection",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "children-in-need-and-child-protection"
                },
                new Topic
                {
                    Id = new Guid("734820b7-f80e-45c3-bb92-960edcc6faa5"),
                    Title = "Children's social work workforce",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "childrens-social-work-workforce"
                },
                new Topic
                {
                    Id = new Guid("17b2e32c-ed2f-4896-852b-513cdf466769"),
                    Title = "Early years foundation stage profile",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "early-years-foundation-stage-profile"
                },
                new Topic
                {
                    Id = new Guid("66ff5e67-36cf-4210-9ad2-632baeb4eca7"),
                    Title = "Looked-after children",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "looked-after-children"
                },
                new Topic
                {
                    Id = new Guid("d5288137-e703-43a1-b634-d50fc9785cb9"),
                    Title = "Secure children's homes",
                    Summary = "",
                    ThemeId = new Guid("cc8e02fd-5599-41aa-940d-26bca68eab53"),
                    Slug = "secure-children-homes"
                },
                new Topic
                {
                    Id = new Guid("0b920c62-ff67-4cf1-89ec-0c74a364e6b4"),
                    Title = "Destinations of key stage 4 and key stage 5 pupils",
                    Summary = "",
                    ThemeId = new Guid("6412a76c-cf15-424f-8ebc-3a530132b1b3"),
                    Slug = "destinations-of-ks4-and-ks5-pupils"
                },
                new Topic
                {
                    Id = new Guid("3bef5b2b-76a1-4be1-83b1-a3269245c610"),
                    Title = "Graduate labour market",
                    Summary = "",
                    ThemeId = new Guid("6412a76c-cf15-424f-8ebc-3a530132b1b3"),
                    Slug = "graduate-labour-market"
                },
                new Topic
                {
                    Id = new Guid("6a0f4dce-ae62-4429-834e-dd67cee32860"),
                    Title = "NEET and participation",
                    Summary = "",
                    ThemeId = new Guid("6412a76c-cf15-424f-8ebc-3a530132b1b3"),
                    Slug = "neet-and-participation"
                },
                new Topic
                {
                    Id = new Guid("4c658598-450b-4493-b972-8812acd154a7"),
                    Title = "Local authority and school finance",
                    Summary = "",
                    ThemeId = new Guid("bc08839f-2970-4f34-af2d-29608a48082f"),
                    Slug = "local-authority-and-school-finance"
                },
                new Topic
                {
                    Id = new Guid("5c5bc908-f813-46e2-aae8-494804a57aa1"),
                    Title = "Student loan forecasts",
                    Summary = "",
                    ThemeId = new Guid("bc08839f-2970-4f34-af2d-29608a48082f"),
                    Slug = "student-loan-forecasts"
                },
                new Topic
                {
                    Id = new Guid("ba0e4a29-92ef-450c-97c5-80a0a6144fb5"),
                    Title = "Advanced learner loans",
                    Summary = "",
                    ThemeId = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Slug = "advanced-learner-loans"
                },
                new Topic
                {
                    Id = new Guid("dd4a5d02-fcc9-4b7f-8c20-c153754ba1e4"),
                    Title = "FE choices",
                    Summary = "",
                    ThemeId = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Slug = "fe-choices"
                },
                new Topic
                {
                    Id = new Guid("88d08425-fcfd-4c87-89da-70b2062a7367"),
                    Title = "Further education and skills",
                    Summary = "",
                    ThemeId = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Slug = "further-education-and-skills"
                },
                new Topic
                {
                    Id = new Guid("cf1f1dc5-27c2-4d15-a55a-9363b7757ff3"),
                    Title = "Further education for benefits claimants",
                    Summary = "",
                    ThemeId = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Slug = "further-education-for-benefits-claimants"
                },
                new Topic
                {
                    Id = new Guid("dc7b7a89-e968-4a7e-af5f-bd7d19c346a5"),
                    Title = "National achievement rates tables",
                    Summary = "",
                    ThemeId = new Guid("92c5df93-c4da-4629-ab25-51bd2920cdca"),
                    Slug = "national-achievement-rates-tables"
                },
                new Topic
                {
                    Id = new Guid("53a1fbb7-5234-435f-892b-9baad4c82535"),
                    Title = "Higher education graduate employment and earnings",
                    Summary = "",
                    ThemeId = new Guid("2ca22e34-b87a-4281-a0eb-b80f4f8dd374"),
                    Slug = "higher-education-graduate-employment-and-earnings"
                },
                new Topic
                {
                    Id = new Guid("2458a916-df6e-4845-9658-a81eace42ffd"),
                    Title = "Higher education statistics",
                    Summary = "",
                    ThemeId = new Guid("2ca22e34-b87a-4281-a0eb-b80f4f8dd374"),
                    Slug = "higher-education-statistics"
                },
                new Topic
                {
                    Id = new Guid("04d95654-9fe0-4f78-9dfd-cf396661ebe9"),
                    Title = "Participation rates in higher education",
                    Summary = "",
                    ThemeId = new Guid("2ca22e34-b87a-4281-a0eb-b80f4f8dd374"),
                    Slug = "participation-rates-in-higher-education"
                },
                new Topic
                {
                    Id = new Guid("7871f559-0cfe-47c0-b48d-25b2bc8a0418"),
                    Title = "Widening participation in higher education",
                    Summary = "",
                    ThemeId = new Guid("2ca22e34-b87a-4281-a0eb-b80f4f8dd374"),
                    Slug = "widening-participation-in-higher-education"
                },
                new Topic
                {
                    Id = new Guid("c9f0b897-d58a-42b0-9d12-ca874cc7c810"),
                    Title = "Admission appeals",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "admission-appeals"
                },
                new Topic
                {
                    Id = new Guid("77941b7d-bbd6-4069-9107-565af89e2dec"),
                    Title = "Exclusions",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "exclusions"
                },
                new Topic
                {
                    Id = new Guid("67c249de-1cca-446e-8ccb-dcdac542f460"),
                    Title = "Pupil absence",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "pupil-absence"
                },
                new Topic
                {
                    Id = new Guid("6b8c0242-68e2-420c-910c-e19524e09cd2"),
                    Title = "Parental responsibility measures",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "parental-responsibility-measures"
                },
                new Topic
                {
                    Id = new Guid("5e196d11-8ac4-4c82-8c46-a10a67c1118e"),
                    Title = "Pupil projections",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "pupil-projections"
                },
                new Topic
                {
                    Id = new Guid("e50ba9fd-9f19-458c-aceb-4422f0c7d1ba"),
                    Title = "School and pupil numbers",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "school-and-pupil-numbers"
                },
                new Topic
                {
                    Id = new Guid("1a9636e4-29d5-4c90-8c07-f41db8dd019c"),
                    Title = "School applications",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "school-applications"
                },
                new Topic
                {
                    Id = new Guid("87c27c5e-ae49-4932-aedd-4405177d9367"),
                    Title = "School capacity",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "school-capacity"
                },
                new Topic
                {
                    Id = new Guid("85349b0a-19c7-4089-a56b-ad8dbe85449a"),
                    Title = "Special educational needs (SEN)",
                    Summary = "",
                    ThemeId = new Guid("ee1855ca-d1e1-4f04-a795-cbd61d326a1f"),
                    Slug = "special-educational-needs"
                },
                new Topic
                {
                    Id = new Guid("85b5454b-3761-43b1-8e84-bd056a8efcd3"),
                    Title = "16 to 19 attainment",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "16-to-19-attainment"
                },
                new Topic
                {
                    Id = new Guid("1e763f55-bf09-4497-b838-7c5b054ba87b"),
                    Title = "GCSEs (key stage 4)",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "gcses-key-stage-4"
                },
                new Topic
                {
                    Id = new Guid("504446c2-ddb1-4d52-bdbc-4148c2c4c460"),
                    Title = "Key stage 1",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "key-stage-1"
                },
                new Topic
                {
                    Id = new Guid("eac38700-b968-4029-b8ac-0eb8e1356480"),
                    Title = "Key stage 2",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "key-stage-2"
                },
                new Topic
                {
                    Id = new Guid("a7ce9542-20e6-401d-91f4-f832c9e58b12"),
                    Title = "Outcome based success measures",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "outcome-based-success-measures"
                },
                new Topic
                {
                    Id = new Guid("1318eb73-02a8-4e50-82a9-7e271176c4d1"),
                    Title = "Performance tables",
                    Summary = "",
                    ThemeId = new Guid("74648781-85a9-4233-8be3-fe6f137165f4"),
                    Slug = "performance-tables"
                },
                new Topic
                {
                    Id = new Guid("0f8792d2-28b1-4537-a1b4-3e139fcf0ca7"),
                    Title = "Initial teacher training (ITT)",
                    Summary = "",
                    ThemeId = new Guid("b601b9ea-b1c7-4970-b354-d1f695c446f1"),
                    Slug = "initial-teacher-training"
                },
                new Topic
                {
                    Id = new Guid("28cfa002-83cb-4011-9ddd-859ec99e0aa0"),
                    Title = "School workforce",
                    Summary = "",
                    ThemeId = new Guid("b601b9ea-b1c7-4970-b354-d1f695c446f1"),
                    Slug = "school-workforce"
                },
                new Topic
                {
                    Id = new Guid("6d434e17-7b76-425d-897d-c7b369b42e35"),
                    Title = "Teacher workforce statistics and analysis",
                    Summary = "",
                    ThemeId = new Guid("b601b9ea-b1c7-4970-b354-d1f695c446f1"),
                    Slug = "teacher-workforce-statistics-and-analysis"
                },
                new Topic
                {
                    Id = new Guid("692050da-9ac9-435a-80d5-a6be4915f0f7"),
                    Title = "UK education and training statistics",
                    Summary = "",
                    ThemeId = new Guid("a95d2ca2-a969-4320-b1e9-e4781112574a"),
                    Slug = "uk-education-and-training-statistics"
                }
            );

            modelBuilder.Entity<Contact>().HasData(
                new Contact
                {
                    Id = new Guid("58117de4-5951-48e4-8537-9f74967a6233"),
                    TeamName = "Test Team",
                    TeamEmail = "explore.statistics@education.gov.uk",
                    ContactName = "Laura Selby",
                    ContactTelNo = "07384237142"
                },
                new Contact
                {
                    Id = new Guid("72f846d7-1580-484e-b299-3ce13070f297"),
                    TeamName = "Another Test Team",
                    TeamEmail = "explore.statistics@education.gov.uk",
                    ContactName = "John Shale",
                    ContactTelNo = "07919937921"
                },
                new Contact
                {
                    Id = new Guid("32d61132-e4c0-442c-88f4-f879971eb699"),
                    TeamName = "Explore Education Statistics",
                    TeamEmail = "explore.statistics@education.gov.uk",
                    ContactName = "Cameron Race",
                    ContactTelNo = "07780991976"
                },
                new Contact
                {
                    Id = new Guid("d246c696-4b3a-4aeb-842c-c1318ee334e8"),
                    TeamName = "School absence and exclusions team",
                    TeamEmail = "schools.statistics@education.gov.uk",
                    ContactName = "Mark Pearson",
                    ContactTelNo = "01142742585"
                },
                new Contact
                {
                    Id = new Guid("74f5aade-6d24-4a0b-be23-2ab4b4b2d191"),
                    TeamName = "School preference statistics team",
                    TeamEmail = "school.preference@education.gov.uk",
                    ContactName = "Helen Bray",
                    ContactTelNo = "02077838553"
                },
                new Contact
                {
                    Id = new Guid("0b63e6c7-5a9d-4c48-b30f-f0729e0644c0"),
                    TeamName = "Special educational needs statistics team",
                    TeamEmail = "sen.statistics@education.gov.uk",
                    ContactName = "Sean Gibson",
                    ContactTelNo = "01325340987"
                },
                new Contact
                {
                    Id = new Guid("0d2ead36-3ebc-482f-a9c9-e17d746a0dd9"),
                    TeamName = "Looked-after children statistics team",
                    TeamEmail = "cla.stats@education.gov.uk",
                    ContactName = "Justin Ushie",
                    ContactTelNo = "01325340817"
                },
                new Contact
                {
                    Id = new Guid("18c9a473-465d-4b8a-b2cf-b24fd3b9c094"),
                    TeamName = "Attainment statistics team",
                    TeamEmail = "Attainment.STATISTICS@education.gov.uk",
                    ContactName = "Raffaele Sasso",
                    ContactTelNo = "07469413581"
                }
            );

            modelBuilder.Entity<Publication>().HasData(
                new Publication
                {
                    Id = new Guid("d63daa75-5c3e-48bf-a232-f232e0d13898"),
                    Title = "30 hours free childcare",
                    Summary = "",
                    TopicId = new Guid("1003fa5c-b60a-4036-a178-e3a69a81b852"),
                    Slug = "30-hours-free-childcare",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-childcare-and-early-years#30-hours-free-childcare")
                },
                new Publication
                {
                    Id = new Guid("79a08466-dace-4ff0-94b6-59c5528c9262"),
                    Title = "Childcare and early years provider survey",
                    Summary = "",
                    TopicId = new Guid("1003fa5c-b60a-4036-a178-e3a69a81b852"),
                    Slug = "childcare-and-early-years-provider-survey",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-childcare-and-early-years#childcare-and-early-years-providers-survey")
                },
                new Publication
                {
                    Id = new Guid("060c5376-35d8-420b-8266-517a9339b7bc"),
                    Title = "Childcare and early years survey of parents",
                    Summary = "",
                    TopicId = new Guid("1003fa5c-b60a-4036-a178-e3a69a81b852"),
                    Slug = "childcare-and-early-years-survey-of-parents",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-childcare-and-early-years#childcare-and-early-years-providers-survey")
                },
                new Publication
                {
                    Id = new Guid("0ce6a6c6-5451-4967-8dd4-2f4fa8131982"),
                    Title = "Education provision: children under 5 years of age",
                    Summary = "",
                    TopicId = new Guid("1003fa5c-b60a-4036-a178-e3a69a81b852"),
                    Slug = "education-provision-children-under-5",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-childcare-and-early-years#provision-for-children-under-5-years-of-age-in-england")
                },
                new Publication
                {
                    Id = new Guid("89869bba-0c00-40f7-b7d6-e28cb904ad37"),
                    Title = "Characteristics of children in need",
                    Summary = "",
                    TopicId = new Guid("22c52d89-88c0-44b5-96c4-042f1bde6ddd"),
                    Slug = "characteristics-of-children-in-need",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-children-in-need#characteristics-of-children-in-need")
                },
                new Publication
                {
                    Id = new Guid("d8baee79-3c88-45f4-b12a-07b91e9b5c11"),
                    Title = "Children's social work workforce",
                    Summary = "",
                    TopicId = new Guid("734820b7-f80e-45c3-bb92-960edcc6faa5"),
                    Slug = "childrens-social-work-workforce",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-childrens-social-care-workforce#statutory-collection")
                },
                new Publication
                {
                    Id = new Guid("fcda2962-82a6-4052-afa2-ea398c53c85f"),
                    Title = "Early years foundation stage profile results",
                    Summary = "",
                    TopicId = new Guid("17b2e32c-ed2f-4896-852b-513cdf466769"),
                    Slug = "early-years-foundation-stage-profile-results",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-early-years-foundation-stage-profile#results-at-national-and-local-authority-level")
                },
                new Publication
                {
                    Id = new Guid("3260801d-601a-48c6-93b7-cf51680323d1"),
                    Title = "Children looked after in England including adoptions",
                    Summary = "",
                    TopicId = new Guid("66ff5e67-36cf-4210-9ad2-632baeb4eca7"),
                    Slug = "children-looked-after-in-england-including-adoptions",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-looked-after-children#looked-after-children")
                },
                new Publication
                {
                    Id = new Guid("f51895df-c682-45e6-b23e-3138ddbfdaeb"),
                    Title = "Outcomes for children looked after by LAs",
                    Summary = "",
                    TopicId = new Guid("66ff5e67-36cf-4210-9ad2-632baeb4eca7"),
                    Slug = "outcomes-for-children-looked-after-by-las",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-looked-after-children#outcomes-for-looked-after-children")
                },
                new Publication
                {
                    Id = new Guid("d7bd5d9d-dc65-4b1d-99b1-4d815b7369a3"),
                    Title = "Children accommodated in secure children's homes",
                    Summary = "",
                    TopicId = new Guid("d5288137-e703-43a1-b634-d50fc9785cb9"),
                    Slug = "children-accommodated-in-secure-childrens-homes",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-secure-children-s-homes")
                },
                new Publication
                {
                    Id = new Guid("8a92c6a5-8110-4c9c-87b1-e15f1c80c66a"),
                    Title = "Destinations of key stage 4 and key stage 5 pupils",
                    Summary = "",
                    TopicId = new Guid("0b920c62-ff67-4cf1-89ec-0c74a364e6b4"),
                    Slug = "destinations-of-ks4-and-ks5-pupils",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-destinations#destinations-after-key-stage-4-and-5")
                },
                new Publication
                {
                    Id = new Guid("42a888c4-9ee7-40fd-9128-f5de546780b3"),
                    Title = "Graduate labour market statistics",
                    Summary = "",
                    TopicId = new Guid("3bef5b2b-76a1-4be1-83b1-a3269245c610"),
                    Slug = "graduate-labour-markets",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/graduate-labour-market-quarterly-statistics#documents")
                },
                new Publication
                {
                    Id = new Guid("a0eb117e-44a8-4732-adf1-8fbc890cbb62"),
                    Title = "Participation in education and training and employment",
                    Summary = "",
                    TopicId = new Guid("6a0f4dce-ae62-4429-834e-dd67cee32860"),
                    Slug = "participation-in-education-training-and-employement",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-neet#participation-in-education")
                },
                new Publication
                {
                    Id = new Guid("2e510281-ca8c-41bf-bbe0-fd15fcc81aae"),
                    Title = "NEET statistics quarterly brief",
                    Summary = "",
                    TopicId = new Guid("6a0f4dce-ae62-4429-834e-dd67cee32860"),
                    Slug = "neet-statistics-quarterly-brief",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-neet#neet:-2016-to-2017-data-")
                },
                new Publication
                {
                    Id = new Guid("8ab47806-e36f-4226-9988-1efe23156872"),
                    Title = "Income and expenditure in academies in England",
                    Summary = "",
                    TopicId = new Guid("4c658598-450b-4493-b972-8812acd154a7"),
                    Slug = "income-and-expenditure-in-academies-in-england",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-local-authority-school-finance-data#academy-spending")
                },
                new Publication
                {
                    Id = new Guid("dcb8b32b-4e50-4fe2-a539-58f9b6b3a366"),
                    Title = "LA and school expenditure",
                    Summary = "",
                    TopicId = new Guid("4c658598-450b-4493-b972-8812acd154a7"),
                    Slug = "la-and-school-expenditure",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-local-authority-school-finance-data#local-authority-and-school-finance")
                },
                new Publication
                {
                    Id = new Guid("94d16c6e-1e5f-48d5-8195-8ea770f1b0d4"),
                    Title = "Planned LA and school expenditure",
                    Summary = "",
                    TopicId = new Guid("4c658598-450b-4493-b972-8812acd154a7"),
                    Slug = "planned-la-and-school-expenditure",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-local-authority-school-finance-data#planned-local-authority-and-school-spending-")
                },
                new Publication
                {
                    Id = new Guid("fd68e147-b7ee-464f-8b02-dcd917dc362d"),
                    Title = "Student loan forecasts for England",
                    Summary = "",
                    TopicId = new Guid("5c5bc908-f813-46e2-aae8-494804a57aa1"),
                    Slug = "student-loan-forecasts-for-england",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-student-loan-forecasts#documents")
                },
                new Publication
                {
                    Id = new Guid("75568912-25ba-499a-8a96-6161b54994db"),
                    Title = "Advanced learner loans applications",
                    Summary = "",
                    TopicId = new Guid("ba0e4a29-92ef-450c-97c5-80a0a6144fb5"),
                    Slug = "advanced-learner-loans-applications",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/further-education#advanced-learner-loans-applications-2017-to-2018")
                },
                new Publication
                {
                    Id = new Guid("f00a784b-52e8-475b-b8ee-dbe730382ba8"),
                    Title = "FE chioces employer satisfaction survey",
                    Summary = "",
                    TopicId = new Guid("dd4a5d02-fcc9-4b7f-8c20-c153754ba1e4"),
                    Slug = "fe-choices-employer-satisfaction-survey",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/fe-choices#employer-satisfaction-survey-data")
                },
                new Publication
                {
                    Id = new Guid("657b1484-0369-4a0e-873a-367b79a48c35"),
                    Title = "FE choices learner satisfaction survey",
                    Summary = "",
                    TopicId = new Guid("dd4a5d02-fcc9-4b7f-8c20-c153754ba1e4"),
                    Slug = "fe-choices-learner-satisfaction-survey",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/fe-choices#learner-satisfaction-survey-data")
                },
                new Publication
                {
                    Id = new Guid("d24783b6-24a7-4ef3-8304-fd07eeedff92"),
                    Title = "Apprenticeship and levy statistics",
                    Summary = "",
                    TopicId = new Guid("88d08425-fcfd-4c87-89da-70b2062a7367"),
                    Slug = "apprenticeship-and-levy-statistics",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/further-education-and-skills-statistical-first-release-sfr#apprenticeships-and-levy---older-data")
                },
                new Publication
                {
                    Id = new Guid("cf0ec981-3583-42a5-b21b-3f2f32008f1b"),
                    Title = "Apprenticeships and traineeships",
                    Summary = "",
                    TopicId = new Guid("88d08425-fcfd-4c87-89da-70b2062a7367"),
                    Slug = "apprenticeships-and-traineeships",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/further-education-and-skills-statistical-first-release-sfr#apprenticeships-and-traineeships---older-data")
                },
                new Publication
                {
                    Id = new Guid("13b81bcb-e8cd-4431-9807-ca588fd1d02a"),
                    Title = "Further education and skills",
                    Summary = "",
                    TopicId = new Guid("88d08425-fcfd-4c87-89da-70b2062a7367"),
                    Slug = "further-education-and-skills",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/further-education-and-skills-statistical-first-release-sfr#fe-and-skills---older-data")
                },
                new Publication
                {
                    Id = new Guid("ce6098a6-27b6-44b5-8e63-36df3a659e69"),
                    Title = "Further education and benefits claimants",
                    Summary = "",
                    TopicId = new Guid("cf1f1dc5-27c2-4d15-a55a-9363b7757ff3"),
                    Slug = "further-education-and-benefits-claimants",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/further-education-for-benefit-claimants#documents")
                },
                new Publication
                {
                    Id = new Guid("7a57d4c0-5233-4d46-8e27-748fbc365715"),
                    Title = "National achievement rates tables",
                    Summary = "",
                    TopicId = new Guid("dc7b7a89-e968-4a7e-af5f-bd7d19c346a5"),
                    Slug = "national-achievement-rates-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/sfa-national-success-rates-tables#national-achievement-rates-tables")
                },
                new Publication
                {
                    Id = new Guid("4d29c28c-efd1-4245-a80c-b55c6a50e3f7"),
                    Title = "Graduate outcomes (LEO)",
                    Summary = "",
                    TopicId = new Guid("53a1fbb7-5234-435f-892b-9baad4c82535"),
                    Slug = "graduate-outcomes",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-higher-education-graduate-employment-and-earnings#documents")
                },
                new Publication
                {
                    Id = new Guid("d4b9551b-d92c-4f98-8731-847780d3c9fa"),
                    Title = "Higher education: destinations of leavers",
                    Summary = "",
                    TopicId = new Guid("2458a916-df6e-4845-9658-a81eace42ffd"),
                    Slug = "higher-education-destinations-of-leavers",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/official-statistics-releases#destinations-of-higher-education-leavers")
                },
                new Publication
                {
                    Id = new Guid("14cfd218-5480-4ba1-a051-5b1e6be14b46"),
                    Title = "Higher education enrolments and qualifications",
                    Summary = "",
                    TopicId = new Guid("2458a916-df6e-4845-9658-a81eace42ffd"),
                    Slug = "higher-education-enrolments-and-qualifications",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/official-statistics-releases#higher-education-enrolments-and-qualifications")
                },
                new Publication
                {
                    Id = new Guid("b83f55db-73fc-46fc-9fda-9b59f5896e9d"),
                    Title = "Performance indicators in higher education",
                    Summary = "",
                    TopicId = new Guid("2458a916-df6e-4845-9658-a81eace42ffd"),
                    Slug = "performance-indicators-in-higher-education",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/official-statistics-releases#performance-indicators")
                },
                new Publication
                {
                    Id = new Guid("6c25a3e9-fc96-472f-895c-9ae4492dd2a4"),
                    Title = "Staff at higher education providers in the UK",
                    Summary = "",
                    TopicId = new Guid("2458a916-df6e-4845-9658-a81eace42ffd"),
                    Slug = "staff-at-higher-education-providers-in-the-uk",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/official-statistics-releases#staff-at-higher-education")
                },
                new Publication
                {
                    Id = new Guid("0c67bbdb-4eb0-41cf-a62e-2589cee58538"),
                    Title = "Participation rates in higher education",
                    Summary = "",
                    TopicId = new Guid("04d95654-9fe0-4f78-9dfd-cf396661ebe9"),
                    Slug = "participation-rates-in-higher-education",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-on-higher-education-initial-participation-rates#participation-rates-in-higher-education-for-england")
                },
                new Publication
                {
                    Id = new Guid("c28f7aca-f1e8-4916-8ce3-fc177b140695"),
                    Title = "Widening participation in higher education",
                    Summary = "",
                    TopicId = new Guid("7871f559-0cfe-47c0-b48d-25b2bc8a0418"),
                    Slug = "widening-participation-in-higher-education",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/widening-participation-in-higher-education#documents")
                },
                new Publication
                {
                    Id = new Guid("123461ab-50be-45d9-8523-c5241a2c9c5b"),
                    Title = "Admission appeals in England",
                    Summary = "",
                    TopicId = new Guid("c9f0b897-d58a-42b0-9d12-ca874cc7c810"),
                    Slug = "admission-appeals-in-england",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-admission-appeals#documents")
                },
                new Publication
                {
                    Id = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Title = "Permanent and fixed-period exclusions in England",
                    MethodologyId = new Guid("c8c911e3-39c1-452b-801f-25bb79d1deb7"),
                    Summary = "",
                    TopicId = new Guid("77941b7d-bbd6-4069-9107-565af89e2dec"),
                    Slug = "permanent-and-fixed-period-exclusions-in-england",
                    NextUpdate = new DateTime(2019, 7, 19),
                    ContactId = new Guid("d246c696-4b3a-4aeb-842c-c1318ee334e8")
                },
                new Publication
                {
                    Id = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Title = "Pupil absence in schools in England",
                    MethodologyId = new Guid("caa8e56f-41d2-4129-a5c3-53b051134bd7"),
                    Summary = "",
                    TopicId = new Guid("67c249de-1cca-446e-8ccb-dcdac542f460"),
                    Slug = "pupil-absence-in-schools-in-england",
                    NextUpdate = new DateTime(2019, 3, 22),
                    DataSource =
                        "[Pupil absence statistics: guide](https://www.gov.uk/government/publications/absence-statistics-guide#)",
                    ContactId = new Guid("d246c696-4b3a-4aeb-842c-c1318ee334e8")
                },
                new Publication
                {
                    Id = new Guid("6c388293-d027-4f74-8d74-29a42e02231c"),
                    Title = "Pupil absence in schools in England: autumn term",
                    Summary = "",
                    TopicId = new Guid("67c249de-1cca-446e-8ccb-dcdac542f460"),
                    Slug = "pupil-absence-in-schools-in-england-autumn-term",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-pupil-absence#autumn-term-release")
                },
                new Publication
                {
                    Id = new Guid("14953fda-02ff-45ed-9573-3a7a0ad8cb10"),
                    Title = "Pupil absence in schools in England: autumn and spring",
                    Summary = "",
                    TopicId = new Guid("67c249de-1cca-446e-8ccb-dcdac542f460"),
                    Slug = "pupil-absence-in-schools-in-england-autumn-and-spring",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-pupil-absence#combined-autumn--and-spring-term-release")
                },
                new Publication
                {
                    Id = new Guid("86af24dc-67c4-47f0-a849-e94c7a1cfe9b"),
                    Title = "Parental responsibility measures",
                    Summary = "",
                    TopicId = new Guid("6b8c0242-68e2-420c-910c-e19524e09cd2"),
                    Slug = "parental-responsibility-measures",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/parental-responsibility-measures#official-statistics")
                },
                new Publication
                {
                    Id = new Guid("aa545525-9ffe-496c-a5b3-974ace56746e"),
                    Title = "National pupil projections",
                    Summary = "",
                    TopicId = new Guid("5e196d11-8ac4-4c82-8c46-a10a67c1118e"),
                    Slug = "national-pupil-projections",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-pupil-projections#documents")
                },
                new Publication
                {
                    Id = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Title = "Schools, pupils and their characteristics",
                    Summary = "",
                    TopicId = new Guid("e50ba9fd-9f19-458c-aceb-4422f0c7d1ba"),
                    Slug = "school-pupils-and-their-characteristics",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-school-and-pupil-numbers")
                },
                new Publication
                {
                    Id = new Guid("66c8e9db-8bf2-4b0b-b094-cfab25c20b05"),
                    MethodologyId = new Guid("8ab41234-cc9d-4b3d-a42c-c9fce7762719"),
                    
                    Title = "Secondary and primary schools applications and offers",
                    Summary = "",
                    TopicId = new Guid("1a9636e4-29d5-4c90-8c07-f41db8dd019c"),
                    Slug = "secondary-and-primary-schools-applications-and-offers",
                    NextUpdate = new DateTime(2019, 6, 14),
                    ContactId = new Guid("74f5aade-6d24-4a0b-be23-2ab4b4b2d191")
                },
                new Publication
                {
                    Id = new Guid("fa591a15-ae37-41b5-98f6-4ce06e5225f4"),
                    Title = "School capacity",
                    Summary = "",
                    TopicId = new Guid("87c27c5e-ae49-4932-aedd-4405177d9367"),
                    Slug = "school-capacity",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-school-capacity#school-capacity-data:-by-academic-year")
                },
                new Publication
                {
                    Id = new Guid("f657afb4-8f4a-427d-a683-15f11a2aefb5"),
                    Title = "Special educational needs in England",
                    Summary = "",
                    TopicId = new Guid("85349b0a-19c7-4089-a56b-ad8dbe85449a"),
                    Slug = "special-educational-needs-in-england",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-special-educational-needs-sen#national-statistics-on-special-educational-needs-in-england")
                },
                new Publication
                {
                    Id = new Guid("30874b87-483a-427e-8916-43cf9020d9a1"),
                    Title = "Special educational needs: analysis and summary of data sources",
                    Summary = "",
                    TopicId = new Guid("85349b0a-19c7-4089-a56b-ad8dbe85449a"),
                    Slug = "special-educational-needs-analysis-and-summary-of-data-sources",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-special-educational-needs-sen#analysis-of-children-with-special-educational-needs")
                },
                new Publication
                {
                    Id = new Guid("88312cc0-fe1d-4ab5-81df-33fd708185cb"),
                    Title = "Statements on SEN and EHC plans",
                    Summary = "",
                    TopicId = new Guid("85349b0a-19c7-4089-a56b-ad8dbe85449a"),
                    Slug = "statements-on-sen-and-ehc-plans",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-special-educational-needs-sen#statements-of-special-educational-needs-(sen)-and-education,-health-and-care-(ehc)-plans")
                },
                new Publication
                {
                    Id = new Guid("1b2fb05c-eb2c-486b-80be-ebd772eda4f1"),
                    Title = "16 to 18 school and college performance tables",
                    Summary = "",
                    TopicId = new Guid("85b5454b-3761-43b1-8e84-bd056a8efcd3"),
                    Slug = "16-to-18-school-and-college-performance-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-attainment-at-19-years#16-to-18-school-and-college-performance-tables")
                },
                new Publication
                {
                    Id = new Guid("3f3a66ec-5777-42ee-b427-8102a14ce0c5"),
                    Title = "A level and other 16 to 18 results",
                    Summary = "",
                    TopicId = new Guid("85b5454b-3761-43b1-8e84-bd056a8efcd3"),
                    Slug = "a-level-and-other-16-to-18-results",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-attainment-at-19-years#a-levels-and-other-16-to-18-results")
                },
                new Publication
                {
                    Id = new Guid("2e95f880-629c-417b-981f-0901e97776ff"),
                    Title = "Level 2 and 3 attainment by young people aged 19",
                    Summary = "",
                    TopicId = new Guid("85b5454b-3761-43b1-8e84-bd056a8efcd3"),
                    Slug = "level-2-and-3-attainment-by-young-people-aged-19",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-attainment-at-19-years#level-2-and-3-attainment")
                },
                new Publication
                {
                    Id = new Guid("bfdcaae1-ce6b-4f63-9b2b-0a1f3942887f"),
                    Title = "GCSE and equivalent results",
                    Summary = "",
                    TopicId = new Guid("1e763f55-bf09-4497-b838-7c5b054ba87b"),
                    Slug = "gcse-and-equivalent-results",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-gcses-key-stage-4#gcse-and-equivalent-results")
                },
                new Publication
                {
                    Id = new Guid("1d0e4263-3d70-433e-bd95-f29754db5888"),
                    Title = "Multi-academy trust performance measures",
                    Summary = "",
                    TopicId = new Guid("1e763f55-bf09-4497-b838-7c5b054ba87b"),
                    Slug = "multi-academy-trust-performance-measures",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-gcses-key-stage-4#multi-academy-trust-performance-measures")
                },
                new Publication
                {
                    Id = new Guid("c8756008-ed50-4632-9b96-01b5ca002a43"),
                    Title = "Revised GCSE and equivalent results in England",
                    Summary = "",
                    TopicId = new Guid("1e763f55-bf09-4497-b838-7c5b054ba87b"),
                    Slug = "revised-gcse-and-equivalent-results-in-england",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-gcses-key-stage-4#gcse-and-equivalent-results,-including-pupil-characteristics")
                },
                new Publication
                {
                    Id = new Guid("9e7e9d5c-b761-43a4-9685-4892392200b7"),
                    Title = "Secondary school performance tables",
                    Summary = "",
                    TopicId = new Guid("1e763f55-bf09-4497-b838-7c5b054ba87b"),
                    Slug = "secondary-school-performance-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-gcses-key-stage-4#secondary-school-performance-tables")
                },
                new Publication
                {
                    Id = new Guid("441a13f6-877c-4f18-828f-119dbd401a5b"),
                    Title = "Phonics screening check and key stage 1 assessments",
                    Summary = "",
                    TopicId = new Guid("504446c2-ddb1-4d52-bdbc-4148c2c4c460"),
                    Slug = "phonics-screening-check-and-ks1-assessments",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-key-stage-1#phonics-screening-check-and-key-stage-1-assessment")
                },
                new Publication
                {
                    Id = new Guid("7ecea655-7b22-4832-b697-26e86769399a"),
                    Title = "Key stage 2 national curriculum test:review outcomes",
                    Summary = "",
                    TopicId = new Guid("eac38700-b968-4029-b8ac-0eb8e1356480"),
                    Slug = "ks2-national-curriculum-test-review-outcomes",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-key-stage-2#key-stage-2-national-curriculum-tests:-review-outcomes")
                },
                new Publication
                {
                    Id = new Guid("eab51107-4ef0-4926-8f8b-c8bd7f5a21d5"),
                    Title = "Multi-academy trust performance measures",
                    Summary = "",
                    TopicId = new Guid("eac38700-b968-4029-b8ac-0eb8e1356480"),
                    Slug = "multi-academy-trust-performance-measures",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-key-stage-2#national-curriculum-assessments-at-key-stage-2")
                },
                new Publication
                {
                    Id = new Guid("10370062-93b0-4dde-9097-5a56bf5b3064"),
                    Title = "National curriculum assessments at key stage 2",
                    Summary = "",
                    TopicId = new Guid("eac38700-b968-4029-b8ac-0eb8e1356480"),
                    Slug = "national-curriculum-assessments-at-ks2",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-key-stage-2#national-curriculum-assessments-at-key-stage-2")
                },
                new Publication
                {
                    Id = new Guid("2434335f-f8e1-41fb-8d6e-4a11bc62b14a"),
                    Title = "Primary school performance tables",
                    Summary = "",
                    TopicId = new Guid("eac38700-b968-4029-b8ac-0eb8e1356480"),
                    Slug = "primary-school-performance-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-key-stage-2#primary-school-performance-tables")
                },
                new Publication
                {
                    Id = new Guid("8b12776b-3d36-4475-8115-00974d7de1d0"),
                    Title = "Further education outcome-based success measures",
                    Summary = "",
                    TopicId = new Guid("a7ce9542-20e6-401d-91f4-f832c9e58b12"),
                    Slug = "further-education-outcome-based-success-measures",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-outcome-based-success-measures#statistics")
                },
                new Publication
                {
                    Id = new Guid("bddcd4b8-db0d-446c-b6e9-03d4230c6927"),
                    Title = "Primary school performance tables",
                    Summary = "",
                    TopicId = new Guid("1318eb73-02a8-4e50-82a9-7e271176c4d1"),
                    Slug = "primary-school-performance-tables-2",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-performance-tables#primary-school-(key-stage-2)")
                },
                new Publication
                {
                    Id = new Guid("263e10d2-b9c3-4e90-a6aa-b52b86de1f5f"),
                    Title = "School and college performance tables",
                    Summary = "",
                    TopicId = new Guid("1318eb73-02a8-4e50-82a9-7e271176c4d1"),
                    Slug = "school-and-college-performance-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-performance-tables#school-and-college:-post-16-(key-stage-5)")
                },
                new Publication
                {
                    Id = new Guid("28aabfd4-a3fb-45e1-bb34-21ca3b7d1aec"),
                    Title = "Secondary school performance tables",
                    Summary = "",
                    TopicId = new Guid("1318eb73-02a8-4e50-82a9-7e271176c4d1"),
                    Slug = "secondary-school-performance-tables",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-performance-tables#secondary-school-(key-stage-4)")
                },
                new Publication
                {
                    Id = new Guid("d34978d5-0317-46bc-9258-13412270ac4d"),
                    Title = "Initial teacher training performance profiles",
                    Summary = "",
                    TopicId = new Guid("0f8792d2-28b1-4537-a1b4-3e139fcf0ca7"),
                    Slug = "initial-teacher-training-performance-profiles",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-teacher-training#performance-data")
                },
                new Publication
                {
                    Id = new Guid("9cc08298-7370-499f-919a-7d203ba21415"),
                    Title = "Initial teacher training: trainee number census",
                    Summary = "",
                    TopicId = new Guid("0f8792d2-28b1-4537-a1b4-3e139fcf0ca7"),
                    Slug = "initial-teacher-training-trainee-number-census",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-teacher-training#census-data")
                },
                new Publication
                {
                    Id = new Guid("3ceb43d0-e705-4cb9-aeb9-cb8638fcbf3d"),
                    Title = "TSM and initial teacher training allocations",
                    Summary = "",
                    TopicId = new Guid("0f8792d2-28b1-4537-a1b4-3e139fcf0ca7"),
                    Slug = "tsm-and-initial-teacher-training-allocations",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/statistics-teacher-training#teacher-supply-model-and-itt-allocations")
                },
                new Publication
                {
                    Id = new Guid("b318967f-2931-472a-93f2-fbed1e181e6a"),
                    Title = "School workforce in England",
                    Summary = "",
                    TopicId = new Guid("28cfa002-83cb-4011-9ddd-859ec99e0aa0"),
                    Slug = "school-workforce-in-england",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-school-workforce#documents")
                },
                new Publication
                {
                    Id = new Guid("d0b47c96-d7de-4d80-9ff7-3bff135d2636"),
                    Title = "Teacher analysis compendium",
                    Summary = "",
                    TopicId = new Guid("6d434e17-7b76-425d-897d-c7b369b42e35"),
                    Slug = "teacher-analysis-compendium",
                    LegacyPublicationUrl =
                        new Uri(
                            "https://www.gov.uk/government/collections/teacher-workforce-statistics-and-analysis#documents")
                },
                new Publication
                {
                    Id = new Guid("2ffbc8d3-eb53-4c4b-a6fb-219a5b95ebc8"),
                    Title = "Education and training statistics for the UK",
                    Summary = "",
                    TopicId = new Guid("692050da-9ac9-435a-80d5-a6be4915f0f7"),
                    Slug = "education-and-training-statistics-for-the-uk",
                    LegacyPublicationUrl =
                        new Uri("https://www.gov.uk/government/collections/statistics-education-and-training#documents")
                }
            );

            modelBuilder.Entity<ReleaseSummary>().HasData(
                    new ReleaseSummary
                    {
                        ReleaseId = new Guid("4fa4fe8e-9a15-46bb-823f-49bf8e0cdec5"),
                        Id = new Guid("1bf7c51f-4d12-4697-8868-455760a887a7")
                    }
                    ,
                    new ReleaseSummary
                    {
                        ReleaseId = new Guid("f75bc75e-ae58-4bc4-9b14-305ad5e4ff7d"),
                        Id = new Guid("51eb730b-d76c-4a0c-aaf2-cf7aa96f133a"),
                    },
                    new ReleaseSummary
                    {
                        ReleaseId = new Guid("e7774a74-1f62-4b76-b9b5-84f14dac7278"),
                        Id = new Guid("06c45b1e-533d-4c95-900b-62beb4620f59"),
                    },
                    new ReleaseSummary
                    {
                        ReleaseId = new Guid("63227211-7cb3-408c-b5c2-40d3d7cb2717"),
                        Id = new Guid("c6e08ed3-d93a-410a-9e7e-600f2cf25725"),
                    }
                )
                ;

            modelBuilder.Entity<ReleaseSummaryVersion>().HasData(
                new ReleaseSummaryVersion
                {
                    Created = new DateTime(2018, 1, 1),
                    Id = new Guid("420ca58e-278b-456b-9031-fe74a6966159"),
                    Slug = "2016-17",
                    ReleaseName = "2016",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.\n\n" +
                        "Find out how and why these statistics are collected and published - [Pupil absence statistics: methodology](../methodology/pupil-absence-in-schools-in-england).",
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    ReleaseSummaryId = new Guid("1bf7c51f-4d12-4697-8868-455760a887a7")
                },
                new ReleaseSummaryVersion
                {
                    Id = new Guid("fe5e8cac-a574-4e83-861b-7b5f927d7d34"),
                    Created = new DateTime(2016, 1, 1),
                    ReleaseName = "2015",
                    Slug = "2015-16",
                    Summary =
                        "Read national statistical summaries and definitions, view charts and tables and download data files across a range of pupil absence subject areas.",
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    ReleaseSummaryId = new Guid("51eb730b-d76c-4a0c-aaf2-cf7aa96f133a"),
                },
                new ReleaseSummaryVersion
                {
                    Id = new Guid("04adfe47-9057-4abd-a0e8-5a6ac56e1560"),
                    Created = new DateTime(2018, 1, 1),
                    ReleaseName = "2016",
                    Slug = "2016-17",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.\n\n" +
                        "Find out how and why these statistics are collected and published - [Permanent and fixed-period exclusion statistics: methodology](../methodology/permanent-and-fixed-period-exclusions-in-england)",
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    ReleaseSummaryId = new Guid("06c45b1e-533d-4c95-900b-62beb4620f59"),
                },
                new ReleaseSummaryVersion
                {
                    Id = new Guid("c6e08ed3-d93a-410a-9e7e-600f2cf25725"),
                    Created = new DateTime(2018, 1, 1),
                    ReleaseName = "2018",
                    Slug = "2018",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.\n\n" +
                        "Find out how and why these statistics are collected and published - [Secondary and primary school applications and offers: methodology](../methodology/secondary-and-primary-schools-applications-and-offers)",
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    ReleaseSummaryId = new Guid("c6e08ed3-d93a-410a-9e7e-600f2cf25725"),
                });
             
            
            modelBuilder.Entity<Release>().HasData(
                //absence
                new Release
                {
                    Id = new Guid("4fa4fe8e-9a15-46bb-823f-49bf8e0cdec5"),
                    ReleaseName = "2016",
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Published = new DateTime(2018, 3, 22),
                    Slug = "2016-17",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.",
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    KeyStatistics = new DataBlock
                    {
                        Id = new Guid("5d1e6b67-26d7-4440-9e77-c0de71a9fc21"),
                        DataBlockRequest = new DataBlockRequest
                        {
                            SubjectId = 1,
                            GeographicLevel = "Country",
                            TimePeriod = new TimePeriod
                            {
                                StartYear = "2012",
                                StartCode = TimeIdentifier.AcademicYear,
                                EndYear = "2016",
                                EndCode = TimeIdentifier.AcademicYear
                            },
                            Filters = new List<string>
                            {
                                FItem(1, FilterItemName.Characteristic__Total),
                                FItem(1, FilterItemName.School_Type__Total)
                            },
                            Indicators = new List<string>
                            {
                                Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                Indicator(1, IndicatorName.Overall_absence_rate),
                                Indicator(1, IndicatorName.Authorised_absence_rate)
                            }
                        },

                        Summary = new Summary
                        {
                            dataKeys = new List<string>
                            {
                                Indicator(1, IndicatorName.Overall_absence_rate),
                                Indicator(1, IndicatorName.Authorised_absence_rate),
                                Indicator(1, IndicatorName.Unauthorised_absence_rate)
                            },
                            dataSummary = new List<string>
                            {
                                "Up from 4.6% in 2015/16",
                                "Similar to previous years",
                                "Up from 1.1% in 2015/16"
                            },
                            description = new MarkDownBlock
                            {
                                Body = " * pupils missed on average 8.2 school days\n" +
                                       " * overall and unauthorised absence rates up on 2015/16\n" +
                                       " * unauthorised absence rise due to higher rates of unauthorised holidays\n" +
                                       " * 10% of pupils persistently absent during 2016/17"
                            }
                        },
                        Tables = new List<Table>
                        {
                            new Table
                            {
                                indicators = new List<string>
                                {
                                    Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                    Indicator(1, IndicatorName.Overall_absence_rate),
                                    Indicator(1, IndicatorName.Authorised_absence_rate)
                                }
                            }
                        },
                        Charts = new List<IContentBlockChart>
                        {
                            new LineChart
                            {
                                Axes = new Dictionary<string, AxisConfigurationItem>
                                {
                                    ["major"] = new AxisConfigurationItem
                                    {
                                        GroupBy = AxisGroupBy.timePeriods,
                                        DataSets = new List<ChartDataSet>
                                        {
                                            new ChartDataSet
                                            {
                                                Indicator = Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                                Filters = new List<string>
                                                {
                                                    FItem(1, FilterItemName.Characteristic__Total),
                                                    FItem(1, FilterItemName.School_Type__Total)
                                                }
                                            },
                                            new ChartDataSet
                                            {
                                                Indicator = Indicator(1, IndicatorName.Overall_absence_rate),
                                                Filters = new List<string>
                                                {
                                                    FItem(1, FilterItemName.Characteristic__Total),
                                                    FItem(1, FilterItemName.School_Type__Total)
                                                }
                                            },
                                            new ChartDataSet
                                            {
                                                Indicator = Indicator(1, IndicatorName.Overall_absence_rate),
                                                Filters = new List<string>
                                                {
                                                    FItem(1, FilterItemName.Characteristic__Total),
                                                    FItem(1, FilterItemName.School_Type__Total)
                                                }
                                            }
                                        },
                                        Title = "School Year"
                                    },
                                    ["minor"] = new AxisConfigurationItem
                                    {
                                        Title = "Absence Rate"
                                    }
                                },
                                Labels = new Dictionary<string, ChartConfiguration>
                                {
                                    [$"{Indicator(1, IndicatorName.Unauthorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                        = new ChartConfiguration
                                        {
                                            Label = "Unauthorised Absence Rate",
                                            Unit = "%",
                                            Colour = "#4763a5",
                                            symbol = ChartSymbol.circle
                                        },
                                    [$"{Indicator(1, IndicatorName.Overall_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                        = new ChartConfiguration
                                        {
                                            Label = "Overall Absence Rate",
                                            Unit = "%",
                                            Colour = "#f5a450",
                                            symbol = ChartSymbol.cross
                                        },
                                    [$"{Indicator(1, IndicatorName.Authorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                        = new ChartConfiguration
                                        {
                                            Label = "Authorised Absence Rate",
                                            Unit = "%",
                                            Colour = "#005ea5",
                                            symbol = ChartSymbol.diamond
                                        }
                                }
                            }
                        }
                    },
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("24c6e9a3-1415-4ca5-9f21-b6b51cb7ba94"),
                            Order = 1, Heading = "About these statistics", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The statistics and data cover the absence of pupils of compulsory school age during the 2016/17 academic year in the following state-funded school types:\n\n" +
                                        "- primary schools\n" +
                                        "- secondary schools\n" +
                                        "- special schools\n\n" +
                                        "They also includes information fo [pupil referral units](../glossary#pupil-referral-unit) and pupils aged 4 years.\n\n" +
                                        "We use the key measures of [overall absence](../glossary#overall-absence) and [persistent absence](../glossary#persistent-absence) to monitor pupil absence and also include [absence by reason](#contents-sections-heading-4) and [pupil characteristics](#contents-sections-heading-6).\n\n" +
                                        "The statistics and data are available at national, regional, local authority (LA) and school level and are used by LAs and schools to compare their local absence rates to regional and national averages for different pupil groups.\n\n" +
                                        "They're also used for policy development as key indicators in behaviour and school attendance policy.\n"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("8965ef44-5ad7-4ab0-a142-78453d6f40af"),
                            Order = 2, Heading = "Pupil absence rates", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Overall absence**\n\n" +
                                        "The [overall absence](../glossary#overall-absence) rate has increased across state-funded primary, secondary and special schools between 2015/16 and 2016/17 driven by an increase in the unauthorised absence rate.\n\n" +
                                        "It increased from 4.6% to 4.7% over this period while the [unauthorised absence](../glossary#unauthorised-absence) rate increased from 1.1% to 1.3%.\n\n" +
                                        "The rate stayed the same at 4% in primary schools but increased from 5.2% to 5.4% for secondary schools. However, in special schools it was much higher and rose to 9.7%.\n\n" +
                                        "The overall and [authorised absence](../glossary#authorised-absence) rates have been fairly stable over recent years after gradually decreasing between 2006/07 and 2013/14."
                                },
                                new DataBlock
                                {
                                    Id = new Guid("5d3058f2-459e-426a-b0b3-9f60d8629fef"),
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 1,
                                        GeographicLevel = "Country",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2012",
                                            StartCode = TimeIdentifier.AcademicYear,
                                            EndYear = "2016",
                                            EndCode = TimeIdentifier.AcademicYear
                                        },
                                        Filters = new List<string>
                                        {
                                            FItem(1, FilterItemName.Characteristic__Total),
                                            FItem(1, FilterItemName.School_Type__Total)
                                        },
                                        Indicators = new List<string>
                                        {
                                            Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                            Indicator(1, IndicatorName.Overall_absence_rate),
                                            Indicator(1, IndicatorName.Authorised_absence_rate)
                                        }
                                    },
                                    Tables = new List<Table>
                                    {
                                        new Table
                                        {
                                            indicators = new List<string>
                                            {
                                                Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                                Indicator(1, IndicatorName.Overall_absence_rate),
                                                Indicator(1, IndicatorName.Authorised_absence_rate)
                                            }
                                        }
                                    },
                                    Charts = new List<IContentBlockChart>
                                    {
                                        new LineChart
                                        {
                                            Axes = new Dictionary<string, AxisConfigurationItem>
                                            {
                                                ["major"] = new AxisConfigurationItem
                                                {
                                                    GroupBy = AxisGroupBy.timePeriods,
                                                    DataSets = new List<ChartDataSet>
                                                    {
                                                        new ChartDataSet
                                                        {
                                                            Indicator = Indicator(1,
                                                                IndicatorName.Unauthorised_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1,
                                                                    FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        },
                                                        new ChartDataSet
                                                        {
                                                            Indicator =
                                                                Indicator(1, IndicatorName.Overall_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1,
                                                                    FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        },
                                                        new ChartDataSet
                                                        {
                                                            Indicator = Indicator(1,
                                                                IndicatorName.Authorised_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1,
                                                                    FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        }
                                                    },
                                                    Title = "School Year"
                                                },
                                                ["minor"] = new AxisConfigurationItem
                                                {
                                                    Title = "Absence Rate"
                                                }
                                            },
                                            Labels = new Dictionary<string, ChartConfiguration>
                                            {
                                                [$"{Indicator(1, IndicatorName.Unauthorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Unauthorised Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#4763a5",
                                                        symbol = ChartSymbol.circle
                                                    },
                                                [$"{Indicator(1, IndicatorName.Overall_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Overall Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#f5a450",
                                                        symbol = ChartSymbol.cross
                                                    },
                                                [$"{Indicator(1, IndicatorName.Authorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Authorised Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#005ea5",
                                                        symbol = ChartSymbol.diamond
                                                    }
                                            }
                                        }
                                    }
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Unauthorised absence**\n\n" +
                                        "The [unauthorised absence](../glossary#unauthorised-absence) rate has not varied much since 2006/07 but is at its highest since records began - 1.3%.\n\n" +
                                        "This is due to an increase in absence due to family holidays not agreed by schools.\n\n" +
                                        "**Authorised absence**\n\n" +
                                        "The [authorised absence](../glossary#authorised-absence) rate has stayed at 3.4% since 2015/16 but has been decreasing in recent years within primary schools.\n\n" +
                                        "**Total number of days missed**\n\n" +
                                        "The total number of days missed for [overall absence](../glossary#overall-absence) across state-funded primary, secondary and special schools has increased to 56.7 million from 54.8 million in 2015/16.\n\n" +
                                        "This partly reflects a rise in the total number of pupils with the average number of days missed per pupil slightly increased to 8.2 days from 8.1 days in 2015/16.\n\n" +
                                        "In 2016/17, 91.8% of primary, secondary and special school pupils missed at least 1 session during the school year - similar to the 91.7% figure from 2015/16."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("6f493eee-443a-4403-9069-fef82e2f5788"),
                            Order = 3, Heading = "Persistent absence", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The [persistent absence](../glossary#persistent-absence) rate increased to and accounted for 37.6% of all absence - up from 36.6% in 2015 to 16 but still down from 43.3% in 2011 to 12.\n\n" +
                                        "It also accounted for almost a third (31.6%) of all [authorised absence](../glossary#authorised-absence) and more than half (53.8%) of all [unauthorised absence](../glossary#unauthorised-absence).\n\n" +
                                        "Overall, it's increased across primary and secondary schools to 10.8% - up from 10.5% in 2015 to 16."
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Persistent absentees**\n\n" +
                                        "The [overall absence](../glossary#overall-absence) rate for persistent absentees across all schools increased to 18.1% - nearly 4 times higher than the rate for all pupils. This is slightly up from 17.6% in 2015/16.\n\n" +
                                        "**Illness absence rate**\n\n" +
                                        "The illness absence rate is almost 4 times higher for persistent absentees at 7.6% compared to 2% for other pupils."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("fbf99442-3b72-46bc-836d-8866c552c53d"),
                            Order = 4, Heading = "Reasons for absence", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "These have been broken down into the following:\n\n" +
                                        "* distribution of absence by reason - the proportion of absence for each reason, calculated by taking the number of absences for a specific reason as a percentage of the total number of absences\n\n" +
                                        "* rate of absence by reason - the rate of absence for each reason, calculated by taking the number of absences for a specific reason as a percentage of the total number of possible sessions\n\n" +
                                        "* one or more sessions missed due to each reason - the number of pupils missing at least 1 session due to each reason"
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Illness**\n\n" +
                                        "This is the main driver behind [overall absence](../glossary#overall-absence) and accounted for 55.3% of all absence - down from 57.3% in 2015/16 and 60.1% in 2014/15.\n\n" +
                                        "While the overall absence rate has slightly increased since 2015/16 the illness rate has stayed the same at 2.6%.\n\n" +
                                        "The absence rate due to other unauthorised circumstances has also stayed the same since 2015/16 at 0.7%.\n\n" +
                                        "**Absence due to family holiday**\n\n" +
                                        "The unauthorised holiday absence rate has increased gradually since 2006/07 while authorised holiday absence rates are much lower than in 2006/07 and remained steady over recent years.\n\n" +
                                        "The percentage of pupils who missed at least 1 session due to family holiday increased to 16.9% - up from 14.7% in 2015/16.\n\n" +
                                        "The absence rate due to family holidays agreed by the school stayed at 0.1%.\n\n" +
                                        "Meanwhile, the percentage of all possible sessions missed due to unauthorised family holidays increased to 0.4% - up from 0.3% in 2015/16.\n\n" +
                                        "**Regulation amendment**\n\n" +
                                        "A regulation amendment in September 2013 stated that term-time leave could only be granted in exceptional circumstances which explains the sharp fall in authorised holiday absence between 2012/13 and 2013/14.\n\n" +
                                        "These statistics and data relate to the period after the [Isle of Wight Council v Jon Platt High Court judgment (May 2016)](https://commonslibrary.parliament.uk/insights/term-time-holidays-supreme-court-judgment/) where the High Court supported a local magistrates’ ruling that there was no case to answer.\n\n" +
                                        "They also partially relate to the period after the April 2017 Supreme Court judgment where it unanimously agreed that no children should be taken out of school without good reason and clarified that 'regularly' means 'in accordance with the rules prescribed by the school'."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("6898538c-3f8d-488d-9e50-12ca7a9fd70c"),
                            Order = 5, Heading = "Distribution of absence", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "Nearly half of all pupils (48.9%) were absent for 5 days or less across primary, secondary and special schools - down from 49.1% in 2015/16.\n\n" +
                                        "The average total absence for primary school pupils was 7.2 days compared to 16.9 days for special school and 9.3 day for secondary school pupils.\n\n" +
                                        "The rate of pupils who had more than 25 days of absence stayed the same as in 2015/16 at 4.3%.\n\n" +
                                        "These pupils accounted for 23.5% of days missed while 8.2% of pupils had no absence.\n\n" +
                                        "**Absence by term**\n\n" +
                                        "Across all schools:\n\n" +
                                        "* [overall absence](../glossary#overall-absence) - highest in summer and lowest in autumn\n\n" +
                                        "* [authorised absence](../glossary#authorised-absence) - highest in spring and lowest in summer\n\n" +
                                        "* [unauthorised absence](../glossary#unauthorised-absence) - highest in summer"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("08b204a2-0eeb-4797-9e0b-a1274e7f6a38"),
                            Order = 6, Heading = "Absence by pupil characteristics", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The [overall absence](../glossary#overall-absence) and [persistent absence](../glossary#persistent-absence) patterns for pupils with different characteristics have been consistent over recent years.\n\n" +
                                        "**Ethnic groups**\n\n" +
                                        "Overall absence rate:\n\n" +
                                        "* Travellers of Irish heritage and Gypsy / Roma pupils - highest at 18.1% and 12.9% respectively\n\n" +
                                        "* Chinese and Black African ethnicity pupils - substantially lower than the national average of 4.7% at 2.4% and 2.9% respectively\n\n" +
                                        "Persistent absence rate:\n\n" +
                                        "* Travellers of Irish heritage pupils - highest at 64%\n\n" +
                                        "* Chinese pupils - lowest at 3.1%\n\n" +
                                        "**Free school meals (FSM) eligibility**\n\n" +
                                        "Overall absence rate:\n\n" +
                                        "* pupils known to be eligible for and claiming FSM - higher at 7.3% compared to 4.2% for non-FSM pupils\n\n" +
                                        "Persistent absence rate:\n\n" +
                                        "* pupils known to be eligible for and claiming FSM - more than double the rate of non-FSM pupils\n\n" +
                                        "**Gender**\n\n" +
                                        "Overall absence rate:\n\n" +
                                        "* boys and girls - very similar at 4.7% and 4.6% respectively\n\n" +
                                        "Persistent absence rate:\n\n" +
                                        "* boys and girls - similar at 10.9% and 10.6% respectively\n\n" +
                                        "**National curriculum year group**\n\n" +
                                        "Overall absence rate:\n\n" +
                                        "* pupils in national curriculum year groups 3 and 4 - lowest at 3.9% and 4% respectively\n\n" +
                                        "* pupils in national curriculum year groups 10 and 11 - highest at 6.1% and 6.2% respectively\n\n" +
                                        "This trend is repeated for the persistent absence rate.\n\n" +
                                        "**Special educational need (SEN)**\n\n" +
                                        "Overall absence rate:\n\n" +
                                        "* pupils with a SEN statement or education healthcare (EHC) plan - 8.2% compared to 4.3% for those with no identified SEN\n\n" +
                                        "Persistent absence rate:\n\n" +
                                        "* pupils with a SEN statement or education healthcare (EHC) plan - more than 2 times higher than pupils with no identified SEN"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("60f8c7ca-faff-4f0d-937d-17fe376461cf"),
                            Order = 7, Heading = "Absence for 4-year-olds", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The [overall absence](../glossary#overall-absence) rate decreased to 5.1% - down from 5.2% for the previous 2 years.\n\n" +
                                        "Absence recorded for 4-year-olds is not treated as authorised or unauthorised and only reported as overall absence."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("d5d604af-6b63-4a51-b106-0c09b8dbedfa"),
                            Order = 8, Heading = "Pupil referral unit absence", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The [overall absence](../glossary#overall-absence) rate increased to 33.9% - up from 32.6% in 2015/16.\n\n" +
                                        "The [persistent absence](../glossary#persistent-absence) rate increased to 73.9% - up from 72.5% in 2015/16."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("68e3028c-1291-42b3-9e7c-9be285dac9a1"),
                            Order = 9, Heading = "Regional and local authority (LA) breakdown", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new DataBlock
                                {
                                    Id = new Guid("4a1af98a-ed8a-438e-92d4-d21cca0429f9"),
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 1,
                                        GeographicLevel = "Local_Authority_District",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2016",
                                            StartCode = TimeIdentifier.AcademicYear,
                                            EndYear = "2017",
                                            EndCode = TimeIdentifier.AcademicYear
                                        },

                                        Indicators = new List<string>
                                        {
                                            Indicator(1, IndicatorName.Unauthorised_absence_rate),
                                            Indicator(1, IndicatorName.Overall_absence_rate),
                                            Indicator(1, IndicatorName.Authorised_absence_rate)
                                        },
                                        Filters = new List<string>
                                        {
                                            FItem(1, FilterItemName.Characteristic__Total),
                                            FItem(1, FilterItemName.School_Type__Total)
                                        }
                                    },
                                    Charts = new List<IContentBlockChart>
                                    {
                                        new MapChart
                                        {
                                            Axes = new Dictionary<string, AxisConfigurationItem>
                                            {
                                                ["major"] = new AxisConfigurationItem
                                                {
                                                    GroupBy = AxisGroupBy.timePeriods,
                                                    DataSets = new List<ChartDataSet>
                                                    {
                                                        new ChartDataSet
                                                        {
                                                            Indicator = Indicator(1,
                                                                IndicatorName.Unauthorised_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1, FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        },
                                                        new ChartDataSet
                                                        {
                                                            Indicator =
                                                                Indicator(1, IndicatorName.Overall_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1, FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        },
                                                        new ChartDataSet
                                                        {
                                                            Indicator = Indicator(1,
                                                                IndicatorName.Authorised_absence_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(1, FilterItemName.Characteristic__Total),
                                                                FItem(1, FilterItemName.School_Type__Total)
                                                            }
                                                        }
                                                    },
                                                    Title = "School Year"
                                                },
                                                ["minor"] = new AxisConfigurationItem
                                                {
                                                    Title = "Absence Rate"
                                                }
                                            },
                                            Labels = new Dictionary<string, ChartConfiguration>
                                            {
                                                [$"{Indicator(1, IndicatorName.Unauthorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Unauthorised Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#4763a5",
                                                        symbol = ChartSymbol.circle
                                                    },
                                                [$"{Indicator(1, IndicatorName.Overall_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Overall Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#f5a450",
                                                        symbol = ChartSymbol.cross
                                                    },
                                                [$"{Indicator(1, IndicatorName.Authorised_absence_rate)}_{FItem(1, FilterItemName.Characteristic__Total)}_{FItem(1, FilterItemName.School_Type__Total)}_____"]
                                                    = new ChartConfiguration
                                                    {
                                                        Label = "Authorised Absence Rate",
                                                        Unit = "%",
                                                        Colour = "#005ea5",
                                                        symbol = ChartSymbol.diamond
                                                    }
                                            }
                                        }
                                    }
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "[Overall absence](../glossary#overall-absence) and [persistent absence](../glossary#persistent-absence) rates vary across primary, secondary and special schools by region and local authority (LA).\n\n" +
                                        "**Overall absence**\n\n" +
                                        "Similar to 2015/16, the 3 regions with the highest rates across all school types were:\n\n" +
                                        "* North East - 4.9%\n\n" +
                                        "* Yorkshire and the Humber - 4.9%\n\n" +
                                        "* South West - 4.8%\n\n" +
                                        "Meanwhile, Inner and Outer London had the lowest rates at 4.4%.\n\n" +
                                        "**Persistent absence**\n\n" +
                                        "The region with the highest persistent absence rate was Yorkshire and the Humber with 11.9% while Outer London had the lowest rate at 10%."
                                }
                            }
                        }
                    }
                },

                // exclusions
                new Release
                {
                    Id = new Guid("e7774a74-1f62-4b76-b9b5-84f14dac7278"),
                    ReleaseName = "2016",
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Published = new DateTime(2018, 7, 19),
                    Slug = "2016-17",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.",
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    KeyStatistics = new DataBlock
                    {
                        Id = new Guid("17a0272b-318d-41f6-bda9-3bd88f78cd3d"),
                        DataBlockRequest = new DataBlockRequest
                        {
                            SubjectId = 12,
                            GeographicLevel = "Country",
                            TimePeriod = new TimePeriod
                            {
                                StartYear = "2012",
                                StartCode = TimeIdentifier.AcademicYear,
                                EndYear = "2016",
                                EndCode = TimeIdentifier.AcademicYear
                            },

                            Filters = new List<string>
                            {
                                FItem(12, FilterItemName.School_Type__Total)
                            },
                            Indicators = new List<string>
                            {
                                Indicator(12, IndicatorName.Number_of_schools),
                                Indicator(12, IndicatorName.Number_of_pupils),
                                Indicator(12, IndicatorName.Number_of_permanent_exclusions),
                                Indicator(12, IndicatorName.Permanent_exclusion_rate),
                                Indicator(12, IndicatorName.Number_of_fixed_period_exclusions),
                                Indicator(12, IndicatorName.Fixed_period_exclusion_rate),
                                Indicator(12, IndicatorName.Percentage_of_pupils_with_fixed_period_exclusions)
                            }
                        },
                        Summary = new Summary
                        {
                            dataKeys = new List<string>
                            {
                                Indicator(12, IndicatorName.Permanent_exclusion_rate),
                                Indicator(12, IndicatorName.Fixed_period_exclusion_rate),
                                Indicator(12, IndicatorName.Number_of_permanent_exclusions)
                            },
                            dataSummary = new List<string>
                            {
                                "Up from 0.08% in 2015/16",
                                "Up from 4.29% in 2015/16",
                                "Up from 6,685 in 2015/16"
                            },
                            description = new MarkDownBlock
                            {
                                Body =
                                    " * overall permanent exclusions rate has increased to 0.10% - up from 0.08% in 2015/16\n" +
                                    " * number of exclusions increased to 7,720 - up from 6,685 in 2015/16\n" +
                                    " * overall fixed-period exclusions rate increased to 4.76% - up from 4.29% in 2015/16\n" +
                                    " * number of exclusions increased to 381,865 - up from 339,360 in 2015/16\n"
                            }
                        },
                        Tables = new List<Table>
                        {
                            new Table
                            {
                                indicators = new List<string>
                                {
                                    Indicator(12, IndicatorName.Permanent_exclusion_rate),
                                    Indicator(12, IndicatorName.Fixed_period_exclusion_rate),
                                    Indicator(12, IndicatorName.Number_of_permanent_exclusions)
                                }
                            }
                        },

                        Charts = new List<IContentBlockChart>
                        {
                            new LineChart
                            {
                                Axes = new Dictionary<string, AxisConfigurationItem>
                                {
                                    ["major"] = new AxisConfigurationItem
                                    {
                                        GroupBy = AxisGroupBy.timePeriods,
                                        DataSets = new List<ChartDataSet>
                                        {
                                            new ChartDataSet
                                            {
                                                Indicator = Indicator(12, IndicatorName.Fixed_period_exclusion_rate),
                                                Filters = new List<string>
                                                {
                                                    FItem(12, FilterItemName.School_Type__Total)
                                                }
                                            },
                                            new ChartDataSet
                                            {
                                                Indicator = Indicator(12,
                                                    IndicatorName.Percentage_of_pupils_with_fixed_period_exclusions),
                                                Filters = new List<string>
                                                {
                                                    FItem(12, FilterItemName.School_Type__Total)
                                                }
                                            }
                                        },
                                        Title = "School Year"
                                    },
                                    ["minor"] = new AxisConfigurationItem
                                    {
                                        Title = "Absence Rate"
                                    }
                                },
                                Labels = new Dictionary<string, ChartConfiguration>
                                {
                                    [$"{Indicator(12, IndicatorName.Fixed_period_exclusion_rate)}_{FItem(12, FilterItemName.School_Type__Total)}_____"]
                                        =
                                        new ChartConfiguration
                                        {
                                            Label = "Fixed period exclusion Rate",
                                            Unit = "%",
                                            Colour = "#4763a5",
                                            symbol = ChartSymbol.circle
                                        },
                                    [$"{Indicator(12, IndicatorName.Percentage_of_pupils_with_fixed_period_exclusions)}_{FItem(12, FilterItemName.School_Type__Total)}_____"]
                                        =
                                        new ChartConfiguration
                                        {
                                            Label = "Pupils with one ore more exclusion",
                                            Unit = "%",
                                            Colour = "#f5a450",
                                            symbol = ChartSymbol.cross
                                        }
                                }
                            }
                        }
                    },
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("b7a968ab-eb49-4100-b133-3d9d94f23d60"),
                            Order = 1, Heading = "About this release", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The statistics and data cover permanent and fixed period exclusions and school-level exclusions during the 2016/17 academic year in the following state-funded school types as reported in the school census:\n\n" +
                                        "* primary schools\n\n" +
                                        "* secondary schools\n\n" +
                                        "* special schools\n\n" +
                                        "They also include national-level information on permanent and fixed-period exclusions for [pupil referral units](../glossary#pupil-referral-unit).\n\n" +
                                        "All figures are based on unrounded data so constituent parts may not add up due to rounding."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("6ed87fd1-81a5-46dc-8841-4598bdae7fee"),
                            Order = 2, Heading = "Permanent exclusions", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The number of [permanent exclusions](../glossary#permanent-exclusion) has increased across all state-funded primary, secondary and special schools to 7,720 - up from 6,685 in 2015/16.\n\n" +
                                        "This works out to an average 40.6 permanent exclusions per day - up from an 35.2 per day in 2015/16.\n\n" +
                                        "The permanent exclusion rate has also increased to 0.10% of pupils - up from from 0.08% in 2015/16 - which is equivalent to around 10 pupils per 10,000."
                                },
                                new DataBlock
                                {
                                    Id = new Guid("dd572e49-87e3-46f5-bb04-e9008573fc91"),
                                    Heading = "Chart showing permanent exclusions in England",
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 12,
                                        GeographicLevel = "Country",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2012",
                                            StartCode = TimeIdentifier.AcademicYear,
                                            EndYear = "2016",
                                            EndCode = TimeIdentifier.AcademicYear
                                        },
                                        Filters = new List<string>
                                        {
                                            FItem(12, FilterItemName.School_Type__Total)
                                        },
                                        Indicators = new List<string>
                                        {
                                            Indicator(12, IndicatorName.Permanent_exclusion_rate),
                                            Indicator(12, IndicatorName.Number_of_pupils),
                                            Indicator(12, IndicatorName.Number_of_permanent_exclusions)
                                        }
                                    },
                                    Tables = new List<Table>
                                    {
                                        new Table
                                        {
                                            indicators = new List<string>
                                            {
                                                Indicator(12, IndicatorName.Number_of_pupils),
                                                Indicator(12, IndicatorName.Number_of_permanent_exclusions),
                                                Indicator(12, IndicatorName.Permanent_exclusion_rate)
                                            }
                                        }
                                    },
                                    Charts = new List<IContentBlockChart>
                                    {
                                        new LineChart
                                        {
                                            Axes = new Dictionary<string, AxisConfigurationItem>
                                            {
                                                ["major"] = new AxisConfigurationItem
                                                {
                                                    GroupBy = AxisGroupBy.timePeriods,
                                                    DataSets = new List<ChartDataSet>
                                                    {
                                                        new ChartDataSet
                                                        {
                                                            Indicator = Indicator(12,
                                                                IndicatorName.Permanent_exclusion_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(12, FilterItemName.School_Type__Total)
                                                            }
                                                        }
                                                    },
                                                    Title = "School Year"
                                                },
                                                ["minor"] = new AxisConfigurationItem
                                                {
                                                    Title = "Exclusion Rate"
                                                }
                                            },
                                            Labels = new Dictionary<string, ChartConfiguration>
                                            {
                                                [$"{Indicator(12, IndicatorName.Permanent_exclusion_rate)}_{FItem(12, FilterItemName.School_Type__Total)}_____"]
                                                    =
                                                    new ChartConfiguration
                                                    {
                                                        Label = "Fixed period exclusion Rate",
                                                        Unit = "%",
                                                        Colour = "#4763a5",
                                                        symbol = ChartSymbol.circle
                                                    }
                                            }
                                        }
                                    }
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "Most occurred in secondary schools which accounted for 83% of all permanent exclusions.\n\n"
                                        +
                                        "The [permanent exclusion](../glossary#permanent-exclusion) rate in secondary schools increased 0.20% - up from from 0.17% in 2015/16 - which is equivalent to 20 pupils per 10,000.\n\n"
                                        +
                                        "The rate also rose in primary schools to 0.03% but decreased in special schools to 0.07% - down from from 0.08% in 2015/16.\n\n"
                                        +
                                        "The rate generally followed a downward trend after 2006/07 - when it stood at 0.12%.\n\n"
                                        +
                                        "However, since 2012/13 it has been on the rise although rates are still lower now than in 2006/07."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("7981db34-afdb-4f84-99e8-bfd43e58f16d"),
                            Order = 3, Heading = "Fixed-period exclusions", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The number of fixed-period exclusionshas increased across all state-funded primary, secondary and special schools to 381,865 - up from 339,360 in 2015/16.\n\n" +
                                        "This works out to around 2,010 fixed-period exclusions per day - up from an 1,786 per day in 2015/16."
                                },
                                new DataBlock
                                {
                                    Id = new Guid("038093a2-0be3-440b-8b22-8116e34aa616"),
                                    Heading = "Chart showing fixed-period exclusions in England",
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 12,
                                        GeographicLevel = "Country",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2012",
                                            StartCode = TimeIdentifier.AcademicYear,
                                            EndYear = "2016",
                                            EndCode = TimeIdentifier.AcademicYear
                                        },

                                        Filters = new List<string>
                                        {
                                            FItem(12, FilterItemName.School_Type__Total)
                                        },
                                        Indicators = new List<string>
                                        {
                                            Indicator(12, IndicatorName.Fixed_period_exclusion_rate),
                                            Indicator(12, IndicatorName.Number_of_pupils),
                                            Indicator(12, IndicatorName.Number_of_fixed_period_exclusions)
                                        }
                                    },
                                    Tables = new List<Table>
                                    {
                                        new Table
                                        {
                                            indicators = new List<string>
                                            {
                                                Indicator(12, IndicatorName.Number_of_pupils),
                                                Indicator(12, IndicatorName.Number_of_fixed_period_exclusions),
                                                Indicator(12, IndicatorName.Fixed_period_exclusion_rate)
                                            }
                                        }
                                    },
                                    Charts = new List<IContentBlockChart>
                                    {
                                        new LineChart
                                        {
                                            Axes = new Dictionary<string, AxisConfigurationItem>
                                            {
                                                ["major"] = new AxisConfigurationItem
                                                {
                                                    GroupBy = AxisGroupBy.timePeriods,
                                                    DataSets = new List<ChartDataSet>
                                                    {
                                                        new ChartDataSet
                                                        {
                                                            Indicator =
                                                                Indicator(12,
                                                                    IndicatorName.Fixed_period_exclusion_rate),
                                                            Filters = new List<string>
                                                            {
                                                                FItem(12, FilterItemName.School_Type__Total)
                                                            }
                                                        }
                                                    },
                                                    Title = "School Year"
                                                },
                                                ["minor"] = new AxisConfigurationItem
                                                {
                                                    Title = "Absence Rate"
                                                }
                                            },
                                            Labels = new Dictionary<string, ChartConfiguration>
                                            {
                                                [$"{Indicator(12, IndicatorName.Fixed_period_exclusion_rate)}_{FItem(12, FilterItemName.School_Type__Total)}_____"]
                                                    =
                                                    new ChartConfiguration
                                                    {
                                                        Label = "Fixed period exclusion Rate",
                                                        Unit = "%",
                                                        Colour = "#4763a5",
                                                        symbol = ChartSymbol.circle
                                                    }
                                            }
                                        }
                                    }
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Primary schools**\n\n" +
                                        "* fixed-period exclusions numbers increased to 64,340 - up from 55,740 in 2015/16\n\n" +
                                        "* fixed-period exclusions rate increased to 1.37% - up from 1.21% in 2015/16\n\n" +
                                        "**Secondary schools**\n\n" +
                                        "* fixed-period exclusions numbers increased to 302,890 - up from 270,135 in 2015/16\n\n" +
                                        "* fixed-period exclusions rate increased to 9.4% - up from 8.46% in 2015/16\n\n" +
                                        "**Special schools**\n\n" +
                                        "* fixed-period exclusions numbers increased to 14,635 - up from 13,485 in 2015/16\n\n" +
                                        "* fixed-period exclusions rate increased to 13.03% - up from 12.53% in 2015/16"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("50e7ca4c-e6c7-4ccd-afc1-93ee4298f358"),
                            Order = 4, Heading = "Number and length of fixed-period exclusions",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Pupils with one or more fixed-period exclusion definition**\n\n" +
                                        "The number of pupils with [one or more fixed-period exclusion](../glossary#one-or-more-fixed-period-exclusion) has increased across state-funded primary, secondary and special schools to 183,475 (2.29% of pupils) up from 167,125 (2.11% of pupils) in 2015/16.\n\n" +
                                        "Of these kinds of pupils, 59.1% excluded on only 1 occasion while 1.5% received 10 or more fixed-period exclusions during the year.\n\n" +
                                        "The percentage of pupils who went on to receive a [permanent exclusion](../glossary#permanent-exclusion) was 3.5%.\n\n" +
                                        "The average length of [fixed-period exclusion](../glossary#fixed-period-exclusion) across schools decreased to 2.1 days - slightly shorter than in 2015/16.\n\n" +
                                        "The highest proportion of fixed-period exclusions (46.6%) lasted for only 1 day.\n\n" +
                                        "Only 2.0% of fixed-period exclusions lasted for longer than 1 week and longer fixed-period exclusions were more prevalent in secondary schools."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("015d0cdd-6630-4b57-9ef3-7341fc3d573e"),
                            Order = 5, Heading = "Reasons for exclusions", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "All reasons (except bullying and theft) saw an increase in [permanent exclusions](../glossary#permanent-exclusion) since 2015/16.\n\n" +
                                        "The following most common reasons saw the largest increases:\n\n" +
                                        "* physical assault against a pupil\n\n" +
                                        "* persistent disruptive behaviour\n\n" +
                                        "* other reasons\n\n" +
                                        "**Persistent disruptive behaviour**\n\n" +
                                        "Remained the most common reason for permanent exclusions accounting for 2,755 (35.7%) of all permanent exclusions - which is equivalent to 3 permanent exclusions per 10,000 pupils.\n\n" +
                                        "However, in special schools the most common reason for exclusion was physical assault against an adult - accounting for 37.8% of all permanent exclusions and 28.1% of all [fixed-period exclusions](../glossary#fixed-period-exclusion).\n\n" +
                                        "Persistent disruptive behaviour is also the most common reason for fixed-period exclusions accounting for 108,640 %) of all fixed-period exclusions - up from 27.7% in 2015/16. This is equivalent to around 135 fixed-period exclusions per 10,000 pupils.\n\n" +
                                        "All reasons saw an increase in fixed-period exclusions since 2015/16. Persistent disruptive behaviour and other reasons saw the largest increases."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("5600ca55-6800-418a-94a5-2f3c3310304e"),
                            Order = 6, Heading = "Exclusions by pupil characteristics", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "There was a similar pattern to previous years where the following groups (where higher exclusion rates are expected) showed an increase in exclusions since 2015/16:\n\n" +
                                        "* boys\n\n" +
                                        "* national curriculum years 9 and 10\n\n" +
                                        "* pupils with special educational needs (SEN)\n\n" +
                                        "* pupils known to be eligible for and claiming free school meals (FSM)\n\n" +
                                        "**Age, national curriculum year group and gender**\n\n" +
                                        "* more than half of all [permanent exclusions](../glossary#permanent-exclusion) (57.2%) and [fixed-period exclusions](../glossary#fixed-period-exclusion) (52.6 %) occur in national curriculum year 9 or above\n\n" +
                                        "* a quarter (25%) of all permanent exclusions were for pupils aged 14 - who also had the highest rates for fixed-period exclusion and pupils receiving [one or more fixed-period exclusion](../glossary#one-or-more-fixed-period-exclusion)\n\n" +
                                        "* the permanent exclusion rate for boys (0.15%) was more than 3 times higher than for girls (0.04%)\n\n" +
                                        "* the fixed-period exclusion rate for boys (6.91%) was almost 3 times higher than for girls (2.53%)\n\n" +
                                        "**Pupils eligible for and claiming free school meals (FSM)**\n\n" +
                                        "* had a permanent exclusion rate of 0.28% and fixed period exclusion rate of 12.54% - around 4 times higher than those not eligible for FSM at 0.07% and 3.50% respectively\n\n" +
                                        "* accounted for 40% of all permanent exclusions and 36.7% of all fixed-period exclusions\n\n" +
                                        "**Special educational needs (SEN) pupils**\n\n" +
                                        "* accounted for around half of all permanent exclusions (46.7%) and fixed-period exclusions (44.9%)\n\n" +
                                        "* had the highest permanent exclusion rate (0.35%0 - 6 times higher than the rate for pupils with no SEN (0.06%)\n\n" +
                                        "* pupils with a statement of SEN or education, health and care (EHC) plan had the highest fixed-period exclusion rate at 15.93% - more than 5 times higher than pupils with no SEN (3.06%)\n\n" +
                                        "**Ethnic group**\n\n" +
                                        "* pupils of Gypsy/Roma and Traveller of Irish Heritage ethnic groups had the highest rates of permanent and fixed-period exclusions - but as the population is relatively small these figures should be treated with some caution\n\n" +
                                        "* pupils from a Black Caribbean background had a permanent exclusion rate nearly 3 times higher (0.28%) than the school population as a whole (0.10%)\n\n" +
                                        "* pupils of Asian ethnic groups had the lowest permanent and fixed-period exclusion rates"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("68f8b290-4b7c-4cac-b0d9-0263609c341b"),
                            Order = 7, Heading = "Independent exclusion reviews", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "There were 560 reviews lodged with [independent review panels](../glossary#independent-review-panel) in maintained primary, secondary and special schools and academies of which 525 (93.4%) were determined and 45 (8.0%) resulted in an offer of reinstatement."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("5708d443-7669-47d8-b6a3-6ad851090710"),
                            Order = 8, Heading = "Pupil referral units exclusions", Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Permanent exclusion**\n\n" +
                                        "The [permanent exclusion](../glossary#permanent-exclusion) rate in [pupil referral units](../glossary#pupil-referral-unit) decreased to 0.13 - down from 0.14% in 2015/16.\n\n" +
                                        "Permanent exclusions rates have remained fairly steady following an increase between 2013/14 and 2014/15.\n\n" +
                                        "**Fixed-period exclusion**\n\n" +
                                        "The [fixed period exclusion](../glossary#fixed-period-exclusion) rate has been steadily increasing since 2013/14.\n\n" +
                                        "The percentage of pupils in pupil referral units who 1 or more fixed-period exclusion increased to 59.17% - up from 58.15% in 2015/16."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("3960ab94-0fad-442c-8aaa-6233eff3bc32"),
                            Order = 9, Heading = "Regional and local authority (LA) breakdown",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "There's considerable variation in the [permanent exclusion](../glossary#permanent-exclusion) and [fixed-period exclusion](../glossary#fixed-period-exclusion) rate at the LA level.\n\n" +
                                        "**Permanent exclusion**\n\n" +
                                        "Similar to 2015/16, the regions with the joint-highest rates across all school types were:\n\n" +
                                        "* North West - 0.14%\n\n" +
                                        "* North West - 0.14%\n\n" +
                                        "Similar to 2015/16, the regions with the lowest rates were:\n\n" +
                                        "* South East - 0.06%\n\n" +
                                        "* Yorkshire and the Humber - 0.07%\n\n" +
                                        "**Fixed-period exclusion**\n\n" +
                                        "Similar to 2015/16, the region with the highest rates across all school types was Yorkshire and the Humber at 7.22% while the lowest rate was in Outer London (3.49%)."
                                }
                            }
                        }
                    }
                },
                // Secondary and primary schools applications offers
                new Release
                {
                    Id = new Guid("63227211-7cb3-408c-b5c2-40d3d7cb2717"),
                    ReleaseName = "2018",
                    PublicationId = new Guid("66c8e9db-8bf2-4b0b-b094-cfab25c20b05"),
                    Published = new DateTime(2018, 6, 14),
                    Slug = "2018",
                    Summary =
                        "Read national statistical summaries, view charts and tables and download data files.",
                    TimePeriodCoverage = TimeIdentifier.AcademicYear,
                    TypeId = new Guid("9d333457-9132-4e55-ae78-c55cb3673d7c"),
                    KeyStatistics = new DataBlock
                    {
                        Id = new Guid("475738b4-ba10-4c29-a50d-6ca82c10de6e"),
                        DataBlockRequest = new DataBlockRequest
                        {
                            SubjectId = 17,
                            GeographicLevel = "Country",
                            TimePeriod = new TimePeriod
                            {
                                StartYear = "2014",
                                StartCode = TimeIdentifier.CalendarYear,
                                EndYear = "2018",
                                EndCode = TimeIdentifier.CalendarYear
                            },
                            Filters = new List<string>
                            {
                                FItem(17, FilterItemName.Year_of_admission__Primary_All_primary)
                            },
                            Indicators = new List<string>
                            {
                                Indicator(17, IndicatorName.Number_of_admissions),
                                Indicator(17, IndicatorName.Number_of_applications_received),
                                Indicator(17, IndicatorName.Number_of_first_preferences_offered),
                                Indicator(17, IndicatorName.Number_of_second_preferences_offered),
                                Indicator(17, IndicatorName.Number_of_third_preferences_offered),
                                Indicator(17, IndicatorName.Number_that_received_one_of_their_first_three_preferences),
                                Indicator(17, IndicatorName.Number_that_received_an_offer_for_a_preferred_school),
                                Indicator(17, IndicatorName.Number_that_received_an_offer_for_a_non_preferred_school),
                                Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer)
                            }
                        },
                        Summary = new Summary
                        {
                            dataKeys = new List<string>
                            {
                                Indicator(17, IndicatorName.Number_of_applications_received),
                                Indicator(17, IndicatorName.Number_of_first_preferences_offered),
                                Indicator(17, IndicatorName.Number_of_second_preferences_offered)
                            },
                            dataSummary = new List<string>
                            {
                                "Down from 620,330 in 2017",
                                "Down from 558,411 in 2017",
                                "Down from 34,792 in 2017"
                            },
                            description = new MarkDownBlock
                            {
                                Body =
                                    "* majority of applicants received a preferred offer\n" +
                                    "* percentage of applicants receiving secondary first choice offers decreases as applications increase\n" +
                                    "* slight proportional increase in applicants receiving primary first choice offer as applications decrease\n"
                            }
                        },
                        Tables = new List<Table>
                        {
                            new Table
                            {
                                indicators = new List<string>
                                {
                                    Indicator(17, IndicatorName.Number_of_applications_received),
                                    Indicator(17, IndicatorName.Number_of_admissions),
                                    Indicator(17, IndicatorName.Number_of_first_preferences_offered),
                                    Indicator(17, IndicatorName.Number_of_second_preferences_offered),
                                    Indicator(17, IndicatorName.Number_of_third_preferences_offered),
                                    Indicator(17,
                                        IndicatorName.Number_that_received_an_offer_for_a_non_preferred_school),
                                    Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer)
                                }
                            }
                        }
                    },
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("def347bd-0b29-405f-a11f-cd03c853a6ed"),
                            Order = 1, Heading = "About this release",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "The statistics and data cover the number of offers made to applicants for primary and secondary school places and the proportion which have received their preferred offers.\n\n" +
                                        "The data was collected from local authorities (LAs) where it was produced as part of the annual applications and offers process for applicants requiring a primary or secondary school place in September 2018.\n\n" +
                                        "The offers were made, and data collected, based on the National Offer Days of 1 March 2018 for secondary schools and 16 April 2018 for primary schools."
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("6bfa9b19-25d6-4d45-8008-9447db541795"),
                            Order = 2, Heading = "Secondary applications and offers",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Secondary applications**\n\n" +
                                        "The number of applications received for secondary school places increased to 582,761 - up 3.6% since 2017. This follows a 2.6% increase between 2016 and 2017.\n\n" +
                                        "This continues the increase in secondary applications seen since 2013 which came on the back of a rise in births which began in the previous decade.\n\n" +
                                        "Since 2013, when secondary applications were at their lowest, there has been a 16.6% increase in the number of applications.\n\n" +
                                        "**Secondary offers**\n\n" +
                                        "The proportion of secondary applicants receiving an offer of their first-choice school has decreased to 82.1% - down from 83.5% in 2017.\n\n" +
                                        "The proportion of applicants who received an offer of any of their preferred schools also decreased slightly to 95.5% - down from 96.1% in 2017.\n\n" +
                                        "**Secondary National Offer Day**\n\n" +
                                        "These statistics come from the process undertaken by local authorities (LAs) which enabled them to send out offers of secondary school places to all applicants on the [Secondary National Offer Day](../glossary#national-offer-day) of 1 March 2018.\n\n" +
                                        "The secondary figures have been collected since 2008 and can be viewed as a time series in the following table."
                                },
                                new DataBlock
                                {
                                    Id = new Guid("52916052-81e3-4b66-80b8-24f8666d9cbf"),
                                    Heading =
                                        "Table of Timeseries of key secondary preference rates, England",
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 17,
                                        GeographicLevel = "Country",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2014",
                                            StartCode = TimeIdentifier.CalendarYear,
                                            EndYear = "2018",
                                            EndCode = TimeIdentifier.CalendarYear
                                        },

                                        Filters = new List<string>
                                        {
                                            FItem(17, FilterItemName.Year_of_admission__Secondary_All_secondary)
                                        },
                                        Indicators = new List<string>
                                        {
                                            Indicator(17,
                                                IndicatorName.Number_that_received_an_offer_for_a_preferred_school),
                                            Indicator(17,
                                                IndicatorName.Number_that_received_an_offer_for_a_non_preferred_school),
                                            Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer),
                                            Indicator(17,
                                                IndicatorName
                                                    .Number_that_received_an_offer_for_a_school_within_their_LA)
                                        }
                                    },
                                    Tables = new List<Table>
                                    {
                                        new Table
                                        {
                                            indicators = new List<string>
                                            {
                                                Indicator(17,
                                                    IndicatorName.Number_that_received_an_offer_for_a_preferred_school),
                                                Indicator(17,
                                                    IndicatorName
                                                        .Number_that_received_an_offer_for_a_non_preferred_school),
                                                Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer)
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("c1f17b4e-f576-40bc-80e1-63767998d080"),
                            Order = 3, Heading = "Secondary geographical variation",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**First preference rates**\n\n" +
                                        "At local authority (LA) level, the 3 highest first preference rates were achieved by the following local authorities:\n\n" +
                                        "* Northumberland - 98.1%\n\n" +
                                        "* East Riding of Yorkshire - 96.7%\n\n" +
                                        "* Bedford - 96.4%\n\n" +
                                        "Northumberland has been the top performer in this measure since 2015.\n\n" +
                                        "As in previous years, the lowest first preference rates were all in London.\n\n" +
                                        "* Hammersmith and Fulham - 51.4%\n\n" +
                                        "* Kensington and Chelsea - 54.3%\n\n" +
                                        "* Lambeth - 55.2%\n\n" +
                                        "These figures do not include City of London which has a tiny number of applications and no secondary schools.\n\n" +
                                        "Hammersmith and Fulham has had the lowest first preference rate since 2015.\n\n" +
                                        "The higher number of practical options available to London applicants and ability to name 6 preferences may encourage parents to make more speculative choices for their top preferences.\n\n" +
                                        "**Regional variation**\n\n" +
                                        "There's much less regional variation in the proportions receiving any preferred offer compared to those for receiving a first preference as shown in the following chart."
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "An applicant can apply for any school, including those situated in another local authority (LA).\n\n" +
                                        "Their authority liaises with the requested school (to make sure the applicant is considered under the admissions criteria) and makes the offer.\n\n" +
                                        "**Secondary offers**\n\n" +
                                        "In 2018, 91.6% of secondary offers made were from schools inside the home authority. This figure has been stable for the past few years.\n\n" +
                                        "This release concentrates on the headline figures for the proportion of children receiving their first preference or a preferred offer.\n\n" +
                                        "However, the main tables provide more information including:\n\n" +
                                        "* the number of places available\n\n" +
                                        "* the proportion of children for whom a preferred offer was not received\n\n" +
                                        "* whether applicants were provided with offers inside or outside their home authority"
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("c3eb66d0-ce13-4e68-861d-98bb914d0814"),
                            Order = 4, Heading = "Primary applications and offers",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Primary applications**\n\n" +
                                        "The number of applications received for primary school places decreased to 608,180 - down 2% on 2017 (620,330).\n\n" +
                                        "This is the result of a notable fall in births since 2013 which is now feeding into primary school applications.\n\n" +
                                        "The number of primary applications is the lowest seen since 2013 - when this data was first collected.\n\n" +
                                        "**Primary offers**\n\n" +
                                        "The proportion of primary applicants receiving an offer of their first-choice school has increased to 91% - up from 90% in 2017.\n\n" +
                                        "The proportion of applicants who received an offer of any of their offer of any of their preferences has also increased slightly to 98.1% - up from 97.7% in 2017.\n\n" +
                                        "**Primary National Offer Day**\n\n" +
                                        "These statistics come from the process undertaken by local authorities (LAs) which enabled them to send out offers of primary school places to all applicants on the Primary National Offer Day of 16 April 2018.\n\n" +
                                        "The primary figures have been collected and published since 2014 and can be viewed as a time series in the following table."
                                },
                                new DataBlock
                                {
                                    Id = new Guid("a8c408ed-45d8-4690-a9f3-2fb0e86377bf"),
                                    Heading =
                                        "Table showing Timeseries of key primary preference rates, England Entry into academic year",
                                    DataBlockRequest = new DataBlockRequest
                                    {
                                        SubjectId = 17,
                                        GeographicLevel = "Country",
                                        TimePeriod = new TimePeriod
                                        {
                                            StartYear = "2014",
                                            StartCode = TimeIdentifier.CalendarYear,
                                            EndYear = "2018",
                                            EndCode = TimeIdentifier.CalendarYear
                                        },

                                        Filters = new List<string>
                                        {
                                            FItem(17, FilterItemName.Year_of_admission__Primary_All_primary)
                                        },
                                        Indicators = new List<string>
                                        {
                                            Indicator(17,
                                                IndicatorName.Number_that_received_an_offer_for_a_preferred_school),
                                            Indicator(17,
                                                IndicatorName.Number_that_received_an_offer_for_a_non_preferred_school),
                                            Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer),
                                            Indicator(17,
                                                IndicatorName
                                                    .Number_that_received_an_offer_for_a_school_within_their_LA)
                                        }
                                    },
                                    Tables = new List<Table>
                                    {
                                        new Table
                                        {
                                            indicators = new List<string>
                                            {
                                                Indicator(17,
                                                    IndicatorName.Number_that_received_an_offer_for_a_preferred_school),
                                                Indicator(17,
                                                    IndicatorName
                                                        .Number_that_received_an_offer_for_a_non_preferred_school),
                                                Indicator(17, IndicatorName.Number_that_did_not_receive_an_offer)
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("b87f2e62-e3e7-4492-9d68-18df8dc29041"),
                            Order = 5, Heading = "Primary geographical variation",
                            Caption = "",
                            Content = new List<IContentBlock>
                            {
                                new MarkDownBlock
                                {
                                    Body =
                                        "**First preference rates**\n\n" +
                                        "At local authority (LA) level, the 3 highest first preference rates were achieved by the following local authorities:\n\n" +
                                        "* East Riding of Yorkshire - 97.6%\n\n" +
                                        "* Northumberland - 97.4%\n\n" +
                                        "* Rutland - 97.4%\n\n" +
                                        "These authorities are in the top 3 for the first time since 2015.\n\n" +
                                        "The lowest first preference rates were all in London.\n\n" +
                                        "* Kensington and Chelsea - 68.4%\n\n" +
                                        "* Camden - 76.5%\n\n" +
                                        "* Hammersmith and Fulham - 76.6%\n\n" +
                                        "Hammersmith and Fulham and Kensington and Chelsea have both been in the bottom 3 since 2015.\n\n" +
                                        "Although overall results are better at primary level than at secondary, for London as a whole the improvement is much more marked:\n\n" +
                                        "* primary first preference rate increased to 86.6% - up from 85.9% in 2017\n\n" +
                                        "* secondary first preference rate decreased to 66% - down from 68.% in 2017"
                                },
                                new MarkDownBlock
                                {
                                    Body =
                                        "**Primary offers**\n\n" +
                                        "In 2018, 97.1% of primary offers made were from schools inside the home authority. This figure has been stable since 2014 when this data was first collected and published.\n\n" +
                                        "As in previous years, at primary level a smaller proportion of offers were made of schools outside the applicant’s home authority compared to secondary level."
                                }
                            }
                        }
                    }
                }
            );
            modelBuilder.Entity<Update>().HasData(
                new Update
                {
                    Id = new Guid("9c0f0139-7f88-4750-afe0-1c85cdf1d047"),
                    ReleaseId = new Guid("4fa4fe8e-9a15-46bb-823f-49bf8e0cdec5"),
                    On = new DateTime(2018, 4, 19),
                    Reason =
                        "Underlying data file updated to include absence data by pupil residency and school location, and updated metadata document."
                },
                new Update
                {
                    Id = new Guid("18e0d40e-bdf7-4c84-99dd-732e72e9c9a5"),
                    ReleaseId = new Guid("4fa4fe8e-9a15-46bb-823f-49bf8e0cdec5"),
                    On = new DateTime(2018, 3, 22),
                    Reason = "First published."
                },
                new Update
                {
                    Id = new Guid("51bd1e2f-2669-4708-b300-799b6be9ec9a"),
                    ReleaseId = new Guid("f75bc75e-ae58-4bc4-9b14-305ad5e4ff7d"),
                    On = new DateTime(2016, 3, 25),
                    Reason = "First published."
                },
                new Update
                {
                    Id = new Guid("4fca874d-98b8-4c79-ad20-d698fb0af7dc"),
                    ReleaseId = new Guid("e7774a74-1f62-4b76-b9b5-84f14dac7278"),
                    On = new DateTime(2018, 7, 19),
                    Reason = "First published."
                },
                new Update
                {
                    Id = new Guid("33ff3f17-0671-41e9-b404-5661ab8a9476"),
                    ReleaseId = new Guid("e7774a74-1f62-4b76-b9b5-84f14dac7278"),
                    On = new DateTime(2018, 8, 25),
                    Reason =
                        "Updated exclusion rates for Gypsy/Roma pupils, to include extended ethnicity categories within the headcount (Gypsy, Roma and other Gypsy/Roma)."
                },
                new Update
                {
                    Id = new Guid("aa4c0f33-cdf4-4df9-9540-18472d46a301"),
                    ReleaseId = new Guid("e3288537-9adb-431d-adfb-9bc3ef7be48c"),
                    On = new DateTime(2018, 6, 13),
                    Reason =
                        "Amended title of table 8e in attachment 'Schools pupils and their characteristics 2018 - LA tables'."
                },
                new Update
                {
                    Id = new Guid("4bd0f73b-ef2b-4901-839a-80cbf8c0871f"),
                    ReleaseId = new Guid("e3288537-9adb-431d-adfb-9bc3ef7be48c"),
                    On = new DateTime(2018, 7, 23),
                    Reason =
                        "Removed unrelated extra material from table 7c in attachment 'Schools pupils and their characteristics 2018 - LA tables'."
                },
                new Update
                {
                    Id = new Guid("7f911a4e-7a56-4f6f-92a6-bd556a9bcfd3"),
                    ReleaseId = new Guid("e3288537-9adb-431d-adfb-9bc3ef7be48c"),
                    On = new DateTime(2018, 9, 5),
                    Reason = "Added cross-border movement local authority level and underlying data tables."
                },
                new Update
                {
                    Id = new Guid("d008b331-af29-4c7e-bb8a-5a2005aa0131"),
                    ReleaseId = new Guid("e3288537-9adb-431d-adfb-9bc3ef7be48c"),
                    On = new DateTime(2018, 9, 11),
                    Reason =
                        "Added open document version of 'Schools pupils and their characteristics 2018 - Cross-border movement local authority tables'."
                },
                new Update
                {
                    Id = new Guid("8900bab9-74ec-4b5d-8be1-648ff4870167"),
                    ReleaseId = new Guid("e7ae88fb-afaf-4d51-a78a-bbb2de671daf"),
                    On = new DateTime(2018, 6, 20),
                    Reason = "First published."
                },
                new Update
                {
                    Id = new Guid("448ca9ea-0cd2-4e6d-b85b-76c3ef7d3bf9"),
                    ReleaseId = new Guid("63227211-7cb3-408c-b5c2-40d3d7cb2717"),
                    On = new DateTime(2018, 6, 14),
                    Reason = "First published."
                }
            );
            modelBuilder.Entity<Link>().HasData(
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("08134c1d-8a58-49a4-8d8b-22e586ffd5ae"),
                    Description = "2008 to 2009",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-in-england-academic-year-2008-to-2009",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("250bace6-aeb9-4fe9-8de2-3a25e0dc717f"),
                    Description = "2009 to 2010",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-from-schools-in-england-academic-year-2009-to-2010",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("a319e4ef-b957-40fb-8a47-b1a97814b220"),
                    Description = "2010 to 2011",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-from-schools-in-england-academic-year-2010-to-2011",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("13acf54a-8016-49ff-9050-c61ebe7acad2"),
                    Description = "2011 to 2012",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-from-schools-in-england-2011-to-2012-academic-year",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("45bd7f62-2018-4a5c-9b93-ccece8e89c46"),
                    Description = "2012 to 2013",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-in-england-2012-to-2013",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("f1225f98-40d5-494c-90f9-99f9fb59ac9d"),
                    Description = "2013 to 2014",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-in-england-2013-to-2014",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("78239816-507d-42b7-98fd-4a71d0d4eb1f"),
                    Description = "2014 to 2015",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-in-england-2014-to-2015",
                },
                new Link
                {
                    PublicationId = new Guid("bf2b4284-6b84-46b0-aaaa-a2e0a23be2a9"),
                    Id = new Guid("564dacdc-f58e-4aa0-8dbd-d8368b4fb6ba"),
                    Description = "2015 to 2016",
                    Url =
                        "https://www.gov.uk/government/statistics/permanent-and-fixed-period-exclusions-in-england-2015-to-2016",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("89c02688-646d-45b5-8919-9a3fafcfe0e9"),
                    Description = "2009 to 2010",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-including-pupil-characteristics-academic-year-2009-to-2010",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("81d91c86-9bf2-496c-b026-9dc255c35635"),
                    Description = "2010 to 2011",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-including-pupil-characteristics-academic-year-2010-to-2011",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("e20141d0-d894-4b8d-a78f-e41c23500786"),
                    Description = "2011 to 2012",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-including-pupil-characteristics",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("ce15f487-87b0-4c07-98f1-6c6732196be7"),
                    Description = "2012 to 2013",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-2012-to-2013",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("75991639-ad77-4ba6-91fc-ac08c00a4ce8"),
                    Description = "2013 to 2014",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-2013-to-2014",
                },
                new Link
                {
                    PublicationId = new Guid("cbbd299f-8297-44bc-92ac-558bcf51f8ad"),
                    Id = new Guid("28e53936-5a52-44be-a7a6-d2f14a426d28"),
                    Description = "2014 to 2015",
                    Url =
                        "https://www.gov.uk/government/statistics/pupil-absence-in-schools-in-england-2014-to-2015",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("dc8b0d8c-08bb-47cc-b3a1-9e6ac9c2c268"),
                    Description = "January 2010",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2010",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("b086ba70-703c-40dd-aaef-d2e19335188e"),
                    Description = "January 2011",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2011",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("181ec43e-cf22-4cab-a128-0a5702468566"),
                    Description = "January 2012",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2012",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("e6b36ee8-ef66-4864-a4b3-9047ee3da338"),
                    Description = "January 2013",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2013",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("398ba8c6-3ea0-49da-8645-ceb3c7fb9860"),
                    Description = "January 2014",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2014",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("5e244416-6f2a-4d22-bea4-c22a229befef"),
                    Description = "January 2015",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2015",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("e3c1db23-8a8f-47fe-b2cd-8e677db700a2"),
                    Description = "January 2016",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2016",
                },
                new Link
                {
                    PublicationId = new Guid("a91d9e05-be82-474c-85ae-4913158406d0"),
                    Id = new Guid("313435b3-fe56-4b92-8e13-670dbf510062"),
                    Description = "January 2017",
                    Url =
                        "https://www.gov.uk/government/statistics/schools-pupils-and-their-characteristics-january-2017",
                }
            );
            modelBuilder.Entity<Methodology>().HasData(
                new Methodology
                {
                    Id = new Guid("caa8e56f-41d2-4129-a5c3-53b051134bd7"),
                    Title = "Pupil absence statistics: methodology",
                    Published = new DateTime(2018, 3, 22),
                    LastUpdated = new DateTime(2019, 6, 26),
                    Summary =
                        "Find out about the methodology behind pupil absence statistics and data and how and why they're collected and published.",
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("5a7fd947-d131-475d-afcd-11ab2b1ece67"),
                            Heading = "1. Overview of absence statistics",
                            Caption = "",
                            Order = 1,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section1.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section1.html",
                                            Encoding.UTF8)
                                        : ""
                                },
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("dabb7562-0433-42fc-96e4-64a68f399dac"),
                            Heading = "2. National Statistics badging",
                            Caption = "",
                            Order = 2,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section2.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section2.html",
                                            Encoding.UTF8)
                                        : ""
                                },
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("50b5031a-93e4-4756-843e-21f88f52ba68"),
                            Heading = "3. Methodology",
                            Caption = "",
                            Order = 3,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section3.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section3.html",
                                            Encoding.UTF8)
                                        : ""
                                },
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("e4ca520f-b609-4abb-a38c-c2d610a18e9f"),
                            Heading = "4. Data collection",
                            Caption = "",
                            Order = 4,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section4.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section4.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("da91d355-b878-4135-a0a9-fb538c601246"),
                            Heading = "5. Data processing",
                            Caption = "",
                            Order = 5,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section5.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section5.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("8df45966-5444-4487-be49-763c5009eea6"),
                            Heading = "6. Data quality",
                            Caption = "",
                            Order = 6,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section6.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section6.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("bf6870de-07d3-4e65-a877-373a63dbcc5d"),
                            Heading = "7. Contacts",
                            Caption = "",
                            Order = 7,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/Section7.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/Section7.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        }
                    },
                    Annexes = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("0522bb29-1e0d-455a-88ef-5887f76fb069"),
                            Heading = "Annex A - Calculations",
                            Caption = "",
                            Order = 1,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexA.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexA.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("f1aac714-665d-436e-a488-1ca409d618bf"),
                            Heading = "Annex B - School attendance codes",
                            Caption = "",
                            Order = 2,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexB.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexB.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("0b888133-215a-4b28-8c24-e0ee9a32df6e"),
                            Heading = "Annex C - Links to pupil absence national statistics and data",
                            Caption = "",
                            Order = 3,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexC.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexC.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("4c4c71e2-24e1-4b57-8a23-ce54fae9b329"),
                            Heading = "Annex D - Standard breakdowns",
                            Caption = "",
                            Order = 4,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexD.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexD.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("97a138bf-4ebb-4b17-86ab-ed78584608e3"),
                            Heading = "Annex E - Timeline",
                            Caption = "",
                            Order = 5,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexE.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexE.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("dc00e749-0893-47f7-8440-5a4da47ceed7"),
                            Heading = "Annex F - Absence rates over time",
                            Caption = "",
                            Order = 6,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Absence_Statistics/AnnexF.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Absence_Statistics/AnnexF.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        }
                    }
                },
                new Methodology
                {
                    Id = new Guid("8ab41234-cc9d-4b3d-a42c-c9fce7762719"),
                    Title = "Secondary and primary school applications and offers: methodology",
                    Published = new DateTime(2018, 6, 14),
                    Summary = "",
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("d82b2a2c-b117-4f96-b812-80de5304ae21"),
                            Heading = "1. Overview of applications and offers statistics",
                            Caption = "",
                            Order = 1,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body =
                                        File.Exists(
                                            @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section1.html")
                                            ? File.ReadAllText(
                                                @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section1.html",
                                                Encoding.UTF8)
                                            : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("f0814433-92d4-4ce5-b63b-2f2cb1b6f48a"),
                            Heading = "2. The admissions process",
                            Caption = "",
                            Order = 2,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body =
                                        File.Exists(
                                            @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section2.html")
                                            ? File.ReadAllText(
                                                @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section2.html",
                                                Encoding.UTF8)
                                            : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("1d7a492b-3e59-4624-9a2a-076635d1f780"),
                            Heading = "3. Methodology",
                            Caption = "",
                            Order = 3,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body =
                                        File.Exists(
                                            @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section3.html")
                                            ? File.ReadAllText(
                                                @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section3.html",
                                                Encoding.UTF8)
                                            : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("f129939b-803f-461b-8838-e7a3d8c6eca2"),
                            Heading = "4. Contacts",
                            Caption = "",
                            Order = 4,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body =
                                        File.Exists(
                                            @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section4.html")
                                            ? File.ReadAllText(
                                                @"Migrations/Html/Secondary_And_Primary_School_Applications_And_Offers/Section4.html",
                                                Encoding.UTF8)
                                            : ""
                                }
                            }
                        }
                    }
                },
                new Methodology
                {
                    Id = new Guid("c8c911e3-39c1-452b-801f-25bb79d1deb7"),
                    Title = "Pupil exclusion statistics: methodology",
                    Published = new DateTime(2018, 8, 25),
                    Summary = "",
                    Content = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("bceaafc1-9548-4a03-98d5-d3476c8b9d99"),
                            Heading = "1. Overview of exclusion statistics",
                            Caption = "",
                            Order = 1,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section1.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section1.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("66b15928-46c6-48d5-90e6-12cf354b4e04"),
                            Heading = "2. National Statistics badging",
                            Caption = "",
                            Order = 2,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section2.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section2.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("863f2b02-67b1-41bd-b1c9-f998f4581297"),
                            Heading = "3. Methodology",
                            Caption = "",
                            Order = 3,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section3.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section3.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("fc66f72e-0176-4c75-b15f-2f35c7329563"),
                            Heading = "4. Data collection",
                            Caption = "",
                            Order = 4,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section4.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section4.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("0c44636a-9a31-4e05-8db7-331ed5eae366"),
                            Heading = "5. Data processing",
                            Caption = "",
                            Order = 5,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section5.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section5.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("69df08b6-dcda-449e-828e-5666c8e6d533"),
                            Heading = "6. Data quality",
                            Caption = "",
                            Order = 6,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section6.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section6.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("fa315759-a51b-4860-8ae5-7b9505873108"),
                            Heading = "7. Contacts",
                            Caption = "",
                            Order = 7,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/Section7.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/Section7.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                    },
                    Annexes = new List<ContentSection>
                    {
                        new ContentSection
                        {
                            Id = new Guid("2bb1ce6d-8b54-4a77-bf7d-466c5f7f6bc3"),
                            Heading = "Annex A - Calculations",
                            Caption = "",
                            Order = 1,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexA.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexA.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("01e9feb8-8ca0-4d98-8a17-78672e4641a7"),
                            Heading = "Annex B - Exclusion by reason codes",
                            Caption = "",
                            Order = 2,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexB.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexB.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("39576875-4a54-4028-bdb0-fecc67041f82"),
                            Heading = "Annex C - Links to pupil exclusions statistics and data",
                            Caption = "",
                            Order = 3,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexC.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexC.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        },
                        new ContentSection
                        {
                            Id = new Guid("e3bfcc04-7d91-45b7-b0ee-19713de4b433"),
                            Heading = "Annex D - Standard breakdowns",
                            Caption = "",
                            Order = 4,
                            Content = new List<IContentBlock>
                            {
                                new HtmlBlock
                                {
                                    Body = File.Exists(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexD.html")
                                        ? File.ReadAllText(@"Migrations/Html/Pupil_Exclusion_Statistics/AnnexD.html",
                                            Encoding.UTF8)
                                        : ""
                                }
                            }
                        }
                    }
                }
            );
        }

        private static string FItem(int subjectId, FilterItemName filterItemName)
        {
            return SubjectFilterItemIds[subjectId][filterItemName].ToString();
        }

        private static string Indicator(int subjectId, IndicatorName indicatorName)
        {
            return SubjectIndicatorIds[subjectId][indicatorName].ToString();
        }
    }
}