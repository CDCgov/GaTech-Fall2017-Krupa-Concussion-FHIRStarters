# Temporary solution to get sample data on FHIR server
1. Run docker-compose up to get the FHIR server up and running.
2. Follow tag-uploader installation instructions: https://github.com/smart-on-fhir/tag-uploader
3. Run the following command replacing bracketed paths: node [path_to_tag_uploader]\tag-uploader -d [path_to_synth_data]\sample_data\synthetic\ -t initial-tag1 -S http://localhost:8080/baseDstu3 --validate

# To generate a tag and push to docker hub 
1. Run the command "docker ps -a" and take note of the container id of the mtbifhir container
2. Run the command "docker commit [mtbifhir_container_id] [repository:tag (e.g. bwells30/mtbi:mtbifhirTestVersion1)]
3. Update the image property under "fhir" in CONCUSSION-AND-TRAUMATIC-BRAIN-INJURY-IN-PEDIATRIC-PATIENTS\docker-compose.yml with the [repository:tag] used in step 2.