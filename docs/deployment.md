# Deployment

The project deployment uses an CI / CD pipeline to continuously deliver development builds to a shared environment and allows for environment promotion via [github actions](https://github.com/DaySmart/scheduling-reservation/actions).

The CI and CD is defined via this [github action workflow](../.github/workflows/ci.yml).

## Caveats

> **IMPORTANT**
> 
> The deployment and execution of the project is dependent on resources that are generated outside of the automation pipeline.
> These resources (DNS and TLS certs) are provisioned manually by an user with appropriate authorization from devops to account delegate via SSO through AWS using the esoteric [Frankenstack](https://github.com/DaySmart/frankenstack) tool.

> **NOTE** This has implications on our running software because our certs may become invalid or our domains may be removed without knowledge of the running application and we have no way to remediate this without manual intervention.

---

### Additional Steps/Gate information coming soon.
