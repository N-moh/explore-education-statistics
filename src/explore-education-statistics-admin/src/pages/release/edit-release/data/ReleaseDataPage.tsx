import ManageReleaseContext, {
  ManageRelease,
} from '@admin/pages/release/ManageReleaseContext';
import Tabs from '@common/components/Tabs';
import TabsSection from '@common/components/TabsSection';
import React, { useContext } from 'react';
import ReleaseDataUploadsSection from './ReleaseDataUploadsSection';
import ReleaseFileUploadsSection from './ReleaseFileUploadsSection';
import ReleaseFootnotesSection from './ReleaseFootnotesSection';

const ReleaseDataPage = () => {
  const { publication, releaseId } = useContext(
    ManageReleaseContext,
  ) as ManageRelease;

  return (
    <>
      <h3>Data uploads</h3>

      <Tabs id="dataUploadTab">
        <TabsSection id="data-upload" title="Data uploads">
          <ReleaseDataUploadsSection
            publicationId={publication.id}
            releaseId={releaseId}
          />
        </TabsSection>
        <TabsSection id="footnotes" title="Footnotes">
          <ReleaseFootnotesSection
            publicationId={publication.id}
            releaseId={releaseId}
          />
        </TabsSection>
        <TabsSection id="file-upload" title="File uploads">
          <ReleaseFileUploadsSection
            publicationId={publication.id}
            releaseId={releaseId}
          />
        </TabsSection>
      </Tabs>
    </>
  );
};

export default ReleaseDataPage;
