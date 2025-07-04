const express = require('express');
const router = express.Router();

/**
 * ひらがな行リストのダミーページ
 * GET /hiragana
 */
router.get('/', (req, res) => {
  const gyo = req.query.gyo || 'あ行';
  res.render('hiragana', { gyo });
});

module.exports = router;
