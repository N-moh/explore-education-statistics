import PrototypeDashboardRelease from '@admin/pages/prototypes/components/PrototypeDashboardRelease';
import React from 'react';

const AdminDashboardInProgress = () => {
  return (
    <>
      {window.location.search === '?status=editNewRelease' && (
        <>
          <h2 className="govuk-heading-m">New releases in progress</h2>
          <ul className="govuk-list-bullet  govuk-!-margin-bottom-9">
            <li>
              <PrototypeDashboardRelease
                title="Pupil absence statistics and data for schools in England"
                years="2017 to 2018"
                editing={window.location.search === '?status=editNewRelease'}
                isNew
                lastEdited={new Date('2019-03-20 17:37')}
                lastEditor={{
                  id: 'me',
                  name: 'me',
                  permissions: {
                    canAccessSystem: false,
                    canAccessPrereleasePages: false,
                    canAccessAnalystPages: false,
                    canAccessUserAdministrationPages: false,
                    canAccessMethodologyAdministrationPages: false,
                  },
                }}
                published={new Date('2019-03-20 09:30')}
              />
            </li>
          </ul>
        </>
      )}
      <h2 className="govuk-heading-m">Editing current releases</h2>
      <ul className="govuk-list-bullet">
        <li>
          <PrototypeDashboardRelease
            title="Permanent and fixed-period exclusions statistics"
            years="2017 to 2018"
            editing
            lastEdited={new Date('2019-03-20 17:37')}
            lastEditor={{
              id: 'me',
              name: 'me',
              permissions: {
                canAccessSystem: false,
                canAccessPrereleasePages: false,
                canAccessAnalystPages: false,
                canAccessUserAdministrationPages: false,
                canAccessMethodologyAdministrationPages: false,
              },
            }}
            published={new Date('2019-03-20 09:30')}
          />
        </li>
      </ul>
    </>
  );
};

export default AdminDashboardInProgress;
