import { Publication, Release } from '@common/services/publicationService';
import React from 'react';
import ContentSubBlockRenderer, {
  SectionToggleHandler,
} from './ContentSubBlockRenderer';

export interface ContentBlockProps {
  content: Release['content'][0]['content'];
  id: string;
  publication: Publication;
  onToggle?: SectionToggleHandler;
}

const ContentBlocks = ({
  content,
  id,
  publication,
  onToggle,
}: ContentBlockProps) => {
  return content && content.length > 0 ? (
    <>
      {content.map((block, index) => {
        const key = `${index}-${block.heading}-${block.type}`;
        return (
          <ContentSubBlockRenderer
            id={id}
            block={block}
            key={key}
            publication={publication}
            onToggle={onToggle}
          />
        );
      })}
    </>
  ) : (
    <div className="govuk-inset-text">
      There is no content for this section.
    </div>
  );
};

export default ContentBlocks;
