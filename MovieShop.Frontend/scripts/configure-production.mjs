import { writeFileSync } from 'node:fs';
const api = process.env.MOVIESHOP_API_URL;
const clientId = process.env.MOVIESHOP_PAYPAL_SANDBOX_CLIENT_ID;
// These credentials are intentionally public for the restricted shared demo user.
const demoUsername = process.env.MOVIESHOP_PUBLIC_DEMO_USERNAME?.trim();
const demoPassword = process.env.MOVIESHOP_PUBLIC_DEMO_PASSWORD;
if (!!demoUsername !== !!demoPassword) throw new Error('Configure both public demo values or neither.');
if (demoUsername && demoUsername !== 'portfolio-demo') throw new Error('Only the dedicated portfolio-demo account may be published.');
const publicDemo = demoUsername && demoPassword ? { username: demoUsername, password: demoPassword } : undefined;
if (!api || !clientId) throw new Error('Set MOVIESHOP_API_URL and MOVIESHOP_PAYPAL_SANDBOX_CLIENT_ID.');
const url = new URL(api);
if (url.protocol !== 'https:' || url.username || url.password || url.search || url.hash || url.pathname !== '/')
  throw new Error('MOVIESHOP_API_URL must be an HTTPS origin without credentials or a path.');
writeFileSync(new URL('../src/environments/environment.production.generated.ts', import.meta.url),
  `// Public build configuration. Shared non-admin demo credentials are intentionally public; never add private secrets.\nexport const productionConfig = ${JSON.stringify({apiUrl: url.origin, payPalClientId: clientId, publicDemo})};\n`);
console.log('Public production configuration generated.');
