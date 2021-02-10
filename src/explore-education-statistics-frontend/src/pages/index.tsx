import React from 'react';
import Link from '../components/Link';
import Page from '../components/Page';

function HomePage() {
  return (
    <Page title="Explore our statistics and data" isHomepage>
      <p className="govuk-body-l">
        Select an option to find the national and regional level statistics and
        data you’re looking for.
      </p>

      <div className="govuk-grid-row dfe-card__container">
        <div className="govuk-grid-column-one-half dfe-card__item">
          <div className="dfe-card">
            <h2 className="govuk-heading-l govuk-!-margin-bottom-0">
              Find statistics and data
            </h2>
            <p className="govuk-!-margin-top-2">
              Browse to find statistical summaries and explanations to help you
              understand and analyse our range of national and regional
              statistics and data.
            </p>
            <Link
              to="/find-statistics"
              data-testid="home--find-statistics-link"
              className="govuk-button govuk-button--start"
            >
              Explore
            </Link>
          </div>
        </div>
        <div className="govuk-grid-column-one-half dfe-card__item">
          <div className="dfe-card">
            <h2 className="govuk-heading-l govuk-!-margin-bottom-0">
              Create your own tables
            </h2>
            <p className="govuk-!-margin-top-2">
              Use our tool to build tables using our range of national and
              regional data.
            </p>
            <Link
              to="/data-tables"
              data-testid="home--table-tool-link"
              className="govuk-button govuk-button--start"
            >
              Create
            </Link>
          </div>
        </div>
      </div>
      <h2 className="govuk-heading-l govuk-!-margin-top-6">
        Supporting information
      </h2>
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-two-thirds">
          <h3 className="govuk-heading-m govuk-!-margin-bottom-1">
            <Link to="/methodology">Education statistics: methodology</Link>
          </h3>
          <p className="govuk-caption-m">
            Browse to find out more about the methodology behind education
            statistics and data and how and why they're collected and published.
          </p>
          <h3 className="govuk-heading-m govuk-!-margin-bottom-1">
            <Link to="/glossary">Education statistics: glossary</Link>
          </h3>
          <p className="govuk-caption-m">
            Browse our A to Z list of definitions for terms used across
            education statistics and data.
          </p>
        </div>
      </div>

      <hr />
      <h3 className="govuk-heading-l govuk-!-margin-top-9">Related services</h3>
      <p className="govuk-body">
        Use these services to find specific performance and other information
        about schools and colleges in England:
      </p>
      <div className="govuk-grid-row govuk-!-margin-bottom-3">
        <div className="govuk-grid-column-one-half">
          <h4 className="govuk-heading-s govuk-!-margin-bottom-0">
            <a
              className="govuk-link"
              href="https://www.gov.uk/school-performance-tables"
            >
              Find and compare schools in England
            </a>
          </h4>
          <p className="govuk-caption-m govuk-!-margin-top-1">
            Search for and check the performance of primary, secondary and
            special needs schools and colleges.
          </p>
        </div>
        <div className="govuk-grid-column-one-half">
          <h4 className="govuk-heading-s govuk-!-margin-bottom-0">
            <a
              className="govuk-link"
              href="https://www.get-information-schools.service.gov.uk/"
            >
              Get information about schools
            </a>
          </h4>
          <p className="govuk-caption-m govuk-!-margin-top-1">
            Search to find and download information about schools, colleges,
            educational organisations and governors in England.
          </p>
        </div>
      </div>
      <div className="govuk-grid-row govuk-!-margin-bottom-9">
        <div className="govuk-grid-column-one-half">
          <h4 className="govuk-heading-s govuk-!-margin-bottom-0">
            <a
              className="govuk-link"
              href="https://schools-financial-benchmarking.service.gov.uk/"
            >
              Schools financial benchmarking
            </a>
          </h4>
          <p className="govuk-caption-m govuk-!-margin-top-1">
            Compare your school's income and expenditure with other schools in
            England.
          </p>
        </div>
        <div className="govuk-grid-column-one-half">
          <h4 className="govuk-heading-s govuk-!-margin-bottom-0">
            <a
              className="govuk-link"
              href="https://www.gov.uk/government/organisations/department-for-education/about/statistics"
            >
              Statistics at DfE
            </a>
          </h4>
          <p className="govuk-caption-m govuk-!-margin-top-1">
            Browse to find and download statistics and data on education and
            children which are not currently available through explore education
            statistics.
          </p>
        </div>
      </div>
      <hr />
      <h3 className="govuk-heading-l govuk-!-margin-top-9">Contact Us</h3>
      <p className="govuk-body govuk-!-margin-top-1">
        If you need help and support or have a question about education
        statistics and data contact:
      </p>

      <p className="govuk-body govuk-!-margin-top-1">
        <strong>Explore education statistics team</strong>
      </p>

      <p className="govuk-caption-m govuk-!-margin-top-1">
        Email
        <br />
        <a href="mailto:explore.statistics@education.gov.uk">
          explore.statistics@education.gov.uk
        </a>
      </p>
    </Page>
  );
}

export default HomePage;
