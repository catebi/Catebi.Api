const express = require('express');
//const axios = require('axios');
const cors = require('cors');

const { Client } = require('@notionhq/client');

require('dotenv').config();
const app = express();
app.use(cors());

const notion = new Client({
  auth: 'secret_entb4diRbuA3yDFpd6s14ClEqn1yCdLENUuUOWLvTok',
});

app.get('/database', async (req, res) => {
  try {
    const allResults = await fetchAllPages('64da38a2d05347f4a9ad1e8c79c8e4f7');
    res.json({ results: allResults });
  } catch (error) {
    console.error('Error querying Notion database:', error);
    res.status(500).send('Internal Server Error');
  }
});

app.listen(3000, () => {
  console.log('Server is running on port 3000');
});

async function fetchAllPages(database_id) {
  let allResults = [];
  let start_cursor = null;

  while (true) {
    var request = {
      database_id,
      filter_properties: ['title', 'S%3Blv', 'Sge%3D'],
    };

    if (start_cursor) {
      request.start_cursor = start_cursor;
    }

    let response = await notion.databases.query(request);

    if (response.has_more) {
      start_cursor = response.next_cursor;
    }

    allResults = allResults.concat(response.results);

    if (!response.has_more) {
      break;
    }
  }

  return allResults;
}
