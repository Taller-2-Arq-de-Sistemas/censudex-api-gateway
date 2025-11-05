
REPOS = \
	censudex-auth \
	censudex-clients \
	# censudex-inventory \
	# censudex-orders \
	# censudex-products

PARENT_DIR := $(abspath $(CURDIR)/..)

# Clones repositories into the parent directory
clone:
	@echo "Cloning Censudex repositories..."
	for repo in $(REPOS); do \
		if [ ! -d "$(PARENT_DIR)/$$repo" ]; then \
			git clone https://github.com/Taller-2-Arq-de-Sistemas/$$repo.git $(PARENT_DIR)/$$repo; \
			cd $(PARENT_DIR)/$$repo && git checkout dev; \
		else \
			echo "Repository $$repo already exists, skipping..."; \
		fi; \
	done

# Copies .env.example to .env on services from their own directory
env:
	@echo "Setting up environment variables..."
	for service in $(REPOS); do \
		if [ -f "$(PARENT_DIR)/$$service/.env.example" ]; then \
			cp $(PARENT_DIR)/$$service/.env.example $(PARENT_DIR)/$$service/.env; \
			echo "Environment file created for $$service"; \
		else \
			echo "No .env.example found for $$service"; \
		fi; \
	done

# Copies .env.example to .env on services from API Gateway directory
propagate-env:
	@echo "Propagating environment variables from API Gateway..."
	@awk -v parent="$(PARENT_DIR)" '\
		BEGIN { current=""; } \
		/^### / { \
			sub(/^### /, "", $$0); \
			current=$$0; \
			gsub(/ /, "-", current); \
			next; \
		} \
		/^[^#].*=/ { \
			if (current != "") { \
				if (current == "API-GATEWAY") { next; } \
				file = parent "/censudex-" tolower(current) "/.env"; \
				print $$0 >> file; \
			} \
		} \
	' .env
	@echo "Environment variables propagated successfully."


# Build and start Docker containers
up:
	@echo "Starting Docker containers..."
	docker compose up -d --build

# Stop all Docker containers
down:
	@echo "Stopping Docker containers..."
	docker compose down

# Full setup (clone + env + up)
setup: clone propagate-env up
	@echo "Censudex environment successfully set up!"

