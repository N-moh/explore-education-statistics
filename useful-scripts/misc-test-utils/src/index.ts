import inquirer from 'inquirer';
import chalk from 'chalk';
import createReleaseAndPublish from './modules/publication/publish';
import createSingleRelease from './modules/release/create';
import uploadSubect from './modules/subject/upload';

const choices = [
  'create new release',
  'publish a new release',
  'upload subjects',
] as const;

const start = async () => {
  const answers = await inquirer.prompt({
    name: 'test',
    type: 'list',
    message: 'What test do you want to run?',
    choices,
    prefix: '>',
  });

  switch (answers.test) {
    case 'create new release':
      await createSingleRelease();
      break;

    case 'publish a new release':
      await createReleaseAndPublish();
      break;

    case 'upload subject':
      // eslint-disable-next-line no-case-declarations
      const release = await inquirer.prompt({
        name: 'id',
        type: 'input',
        message: 'Release ID from existing publication',
        prefix: '>',
      });

      await uploadSubect(release.id);
      break;

    default:
      // eslint-disable-next-line no-console
      console.error(chalk.red('Invalid action:', answers.test));
  }
};
start();
