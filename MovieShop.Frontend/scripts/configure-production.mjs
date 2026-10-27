import { writeFileSync } from 'node:fs';
const api = process.env.MOVIESHOP_API_URL;
const clientId = process.env.MOVIESHOP_PAYPAL_SANDBOX_CLIENT_ID;
if (!api || !clientId) throw new Error('Set MOVIESHOP_API_URL and MOVIESHOP_PAYPAL_SANDBOX_CLIENT_ID.');
const url = new URL(api);
if (url.protocol !== 'https:' || url.username || url.password || url.search || url.hash || url.pathname !== '/')
  throw new Error('MOVIESHOP_API_URL must be an HTTPS origin without credentials or a path.');
writeFileSync(new URL('../src/environments/environment.production.generated.ts', import.meta.url),
  `// Generated from public build configuration. Never put secrets here.\nexport const productionConfig = ${JSON.stringify({apiUrl: url.origin, payPalClientId: clientId})};\n`);
console.log('Public production configuration generated.');
